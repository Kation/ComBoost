using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.AspNetCore
{
    public abstract class DomainServiceDescriptor
    {
        public abstract Task<IDomainExecutionContext> ExecuteAsync(string methodName, IDomainContext context);

        public abstract bool ContainsMethod(string methodName);
    }

    public class DomainServiceDescriptor<T> : DomainServiceDescriptor
        where T : class, IDomainService
    {
        //private Dictionary<string, Tuple<MethodInfo, Func<IDomainService, object[], Task>, Func<Task, object>>> _Caches = new Dictionary<string, Tuple<MethodInfo, Func<IDomainService, object[], Task>, Func<Task, object>>>();
        private static readonly Dictionary<string, Func<IDomainService, IDomainContext, IValueProvider, Task>> _Caches = new Dictionary<string, Func<IDomainService, IDomainContext, IValueProvider, Task>>();

        static DomainServiceDescriptor()
        {
            var type = typeof(T);
            _Caches = type.GetMethods().Where(method => !method.IsSpecialName && typeof(Task).IsAssignableFrom(method.ReturnType) && !method.IsGenericMethodDefinition).ToDictionary(t => t.Name.ToLower(), method =>
            {
                var invoker = DomainServiceInvokerBuilder<T>.GetInvokerReference(method);
                var serviceInput = Expression.Parameter(typeof(IDomainService), "serviceInput");
                var contextInput = Expression.Parameter(typeof(IDomainContext), "contextInput");
                var valueProviderInput = Expression.Parameter(typeof(IValueProvider), "valueProvider");
                var parameters = method.GetParameters();
                var values = parameters.Select(t => Expression.Variable(t.ParameterType, t.Name + "Value")).ToArray();
                var serviceVariable = Expression.Variable(type, "service");
                var invokerVariable = Expression.Variable(invoker.Type, "invoker");
                var i = 0;
                List<Expression> blocks = parameters.Select(parameter =>
                {
                    Expression expression;
                    var fromAttribute = parameter.GetCustomAttributes().Where(t => t is FromAttribute).Cast<FromAttribute>().FirstOrDefault();
                    if (fromAttribute == null)
                    {
                        expression = Expression.Call(valueProviderInput, typeof(IValueProvider).GetMethod(nameof(IValueProvider.GetValue))!, Expression.Constant(parameter.Name, typeof(string)), Expression.Constant(parameter.ParameterType, typeof(Type)));
                    }
                    else
                    {
                        expression = Expression.Call(Expression.Constant(fromAttribute), typeof(FromAttribute).GetMethod(nameof(FromAttribute.GetValue))!, contextInput, Expression.Constant(parameter, typeof(ParameterInfo)));
                    }
                    if (parameter.ParameterType.IsValueType)
                        expression = Expression.Unbox(expression, parameter.ParameterType);
                    else
                        expression = Expression.Convert(expression, parameter.ParameterType);
                    expression = Expression.Assign(values[i], expression);
                    i++;
                    return expression;
                }).ToList();
                blocks.Add(Expression.Assign(invokerVariable, Expression.New(invoker.Constructor, Expression.Convert(serviceInput, typeof(T)), contextInput)));
                blocks.Add(Expression.Call(invokerVariable, invoker.InvokeMethod, values));
                var executeLambda = Expression.Lambda<Func<IDomainService, IDomainContext, IValueProvider, Task>>(
                    Expression.Block(values.Append(serviceVariable).Append(invokerVariable), blocks)
                    , serviceInput, contextInput, valueProviderInput);
                return executeLambda.Compile();
            });
        }

        public override bool ContainsMethod(string methodName)
        {
            return _Caches.ContainsKey(methodName.ToLower());
        }

        public override async Task<IDomainExecutionContext> ExecuteAsync(string methodName, IDomainContext context)
        {
            methodName = methodName.ToLower();
            if (!_Caches.TryGetValue(methodName, out var invoker))
                throw new InvalidOperationException("不存在的方法名。");
            var service = context.GetRequiredService<T>();
            await invoker(service, context, context.GetRequiredService<IValueProvider>());
            return service.Context!;
        }
    }
}
