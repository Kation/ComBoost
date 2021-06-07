using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.AspNetCore
{
    public abstract class DomainServiceDescriptor
    {
        IReadOnlyCollection<IDomainServiceFilter> Filters { get; }

        public abstract Task<IDomainExecutionContext> ExecuteAsync(string methodName, IDomainContext context);

        public abstract bool ContainsMethod(string methodName);
    }

    public class DomainServiceDescriptor<T> : DomainServiceDescriptor
        where T : class, IDomainService
    {
        private Dictionary<string, Tuple<MethodInfo, Func<IDomainService, object[], Task>, Func<Task, object>>> _Caches = new Dictionary<string, Tuple<MethodInfo, Func<IDomainService, object[], Task>, Func<Task, object>>>();
        private IDomainServiceFilter[] _ClassFilters;
        private Dictionary<string, IDomainServiceFilter[]> _MethodFilters;

        public DomainServiceDescriptor()
        {
            var type = typeof(T);
            _MethodFilters = new Dictionary<string, IDomainServiceFilter[]>();
            foreach (var method in type.GetMethods())
            {
                if (method.IsSpecialName)
                    continue;
                if (!typeof(Task).IsAssignableFrom(method.ReturnType))
                    continue;
                if (method.IsGenericMethodDefinition)
                    continue;
                if (_Caches.ContainsKey(method.Name))
                    throw new NotSupportedException("领域服务方法不允许重载方法。");

                var serviceInput = Expression.Parameter(typeof(IDomainService), "serviceInput");
                var valuesInput = Expression.Parameter(typeof(object[]), "valuesInput");
                var serviceVariable = Expression.Variable(type, "service");
                var i = 0;
                var blocks = Expression.Block(typeof(Task), new ParameterExpression[] { serviceVariable },
                    Expression.Assign(serviceVariable, Expression.Convert(serviceInput, type)),
                    Expression.Call(serviceVariable, method,
                        method.GetParameters().Select(t =>
                        {
                            var value = Expression.ArrayAccess(valuesInput, Expression.Constant(i));
                            i++;
                            if (t.ParameterType.IsValueType)
                                return Expression.Unbox(value, t.ParameterType);
                            else
                                return Expression.Convert(value, t.ParameterType);
                        }))
                    );
                var executeLambda = Expression.Lambda<Func<IDomainService, object[], Task>>(blocks, serviceInput, valuesInput);
                var executeFunc = executeLambda.Compile();

                Func<Task, object> resultFunc = null;
                if (method.ReturnType.IsGenericType)
                {
                    var taskInput = Expression.Parameter(typeof(Task), "task");

                    var taskVariable = Expression.Variable(method.ReturnType, "convertedTask");

                    var b = Expression.Block(typeof(object), new ParameterExpression[] { taskVariable },
                        Expression.Assign(taskVariable, Expression.Convert(taskInput, method.ReturnType)),
                        Expression.Call(Expression.Call(taskVariable, method.ReturnType.GetMethod("GetAwaiter")), method.ReturnType.GetMethod("GetAwaiter").ReturnType.GetMethod("GetResult")));

                    var resultLambda = Expression.Lambda<Func<Task, object>>(b, taskInput);
                    resultFunc = resultLambda.Compile();
                }
                _Caches.Add(method.Name.ToLower(), new Tuple<MethodInfo, Func<IDomainService, object[], Task>, Func<Task, object>>(method, executeFunc, resultFunc));
                _MethodFilters.Add(method.Name, method.GetCustomAttributes().Where(t => t is IDomainServiceFilter).Cast<IDomainServiceFilter>().ToArray());
            }
            _ClassFilters = type.GetCustomAttributes().Where(t => t is IDomainServiceFilter).Cast<IDomainServiceFilter>().ToArray();
        }

        public override bool ContainsMethod(string methodName)
        {
            return _Caches.ContainsKey(methodName.ToLower());
        }

        public override async Task<IDomainExecutionContext> ExecuteAsync(string methodName, IDomainContext context)
        {
            methodName = methodName.ToLower();
            if (!_Caches.TryGetValue(methodName, out var value))
                throw new InvalidOperationException("不存在的方法名。");
            var service = context.GetRequiredService<T>();

            var globalFilters = context.GetService<IOptions<DomainFilterOptions>>()?.Value.Filters;
            var filterFactory = context.GetService<IOptionsFactory<DomainFilterOptions<T>>>();
            var serviceFilters = filterFactory?.Create(null).Filters;
            var methodAttributeFilters = _MethodFilters[value.Item1.Name];
            var methodFilters = filterFactory?.Create(value.Item1.Name).Filters;

            DomainExecutionContext executionContext = new DomainExecutionContext(service, context, value.Item1);
            service.Initialize(executionContext);
            DomainExecutionPipeline pipeline = async () =>
            {
                var task = value.Item2(service, executionContext.ParameterValues);
                await task;
                if (value.Item3 == null)
                    executionContext.Done();
                else
                {
                    executionContext.Done(value.Item3(task));
                }
            };

            pipeline = MakePipleline(pipeline, executionContext, context.Filter.ToArray());
            if (methodFilters != null)
                pipeline = MakePipleline(pipeline, executionContext, methodFilters);
            if (methodAttributeFilters.Length > 0)
                pipeline = MakePipleline(pipeline, executionContext, methodAttributeFilters);
            if (serviceFilters != null)
                pipeline = MakePipleline(pipeline, executionContext, serviceFilters);
            if (_ClassFilters.Length > 0)
                pipeline = MakePipleline(pipeline, executionContext, _ClassFilters);
            if (globalFilters != null)
                pipeline = MakePipleline(pipeline, executionContext, globalFilters);

            await pipeline();

            return executionContext;
        }

        private DomainExecutionPipeline MakePipleline(DomainExecutionPipeline pipeline, DomainExecutionContext context, IReadOnlyList<IDomainServiceFilter> filters)
        {
            for (int i = filters.Count - 1; i >= 0; i--)
            {
                var next = pipeline;
                var index = i;
                pipeline = () => filters[index].OnExecutionAsync(context, next);
            }
            return pipeline;
        }
    }
}
