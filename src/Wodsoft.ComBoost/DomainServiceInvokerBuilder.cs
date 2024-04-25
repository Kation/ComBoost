using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    public class DomainServiceInvokerBuilder
    {
        protected internal static readonly MethodInfo _GetMethodMethod = typeof(MethodBase).GetMethod("GetMethodFromHandle", BindingFlags.Public | BindingFlags.Static, null, new[] { typeof(RuntimeMethodHandle), typeof(RuntimeTypeHandle) }, null);
        protected internal static readonly ConstructorInfo _ExecutionContextConstructor = typeof(DomainExecutionContext).GetConstructor(new Type[] { typeof(IDomainService), typeof(IDomainContext), typeof(MethodInfo), typeof(object[]) });
        protected internal static readonly MethodInfo _Initialize = typeof(IDomainService).GetMethod("Initialize");

        public abstract class DomainServiceInvoker<TDomainService>
            where TDomainService : class, IDomainService
        {
            protected IDomainExecutionContext? _executionContext;

            protected DomainServiceInvoker(TDomainService domainService, IDomainContext domainContext)
            {
                DomainService = domainService;
                DomainContext = domainContext;
                var events = domainContext.GetService<IOptions<DomainServiceEventManagerOptions<TDomainService>>>().Value.GetEvents();
                foreach (var e in events)
                    domainContext.EventManager.AddEventHandler(e.Key, e.Value);
            }

            public TDomainService DomainService { get; }

            public IDomainContext DomainContext { get; }

            private static Dictionary<string, List<IDomainServiceFilter>>? _Filters;

            protected abstract Task ExecuteAsync();

            protected virtual IList<IDomainServiceFilter> GetFilters()
            {
                if (_Filters == null)
                    _Filters = typeof(TDomainService).GetMethods(BindingFlags.Public | BindingFlags.Instance).Where(t => !t.IsSpecialName && t.ReturnType == typeof(Task) || ((t.ReturnType.IsGenericType && t.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))))
                        .ToDictionary(t => t.Name, t =>
                        {
                            List<IDomainServiceFilter> filters = new List<IDomainServiceFilter>();
                            filters.AddRange(DomainContext.GetService<IOptions<DomainFilterOptions>>().Value.Filters);
                            filters.AddRange(typeof(TDomainService).GetCustomAttributes().OfType<IDomainServiceFilter>());
                            filters.AddRange(DomainContext.GetService<IOptions<DomainFilterOptions<TDomainService>>>().Value.Filters);
                            filters.AddRange(t.GetCustomAttributes().OfType<IDomainServiceFilter>());
                            filters.AddRange(DomainContext.GetService<IOptionsMonitor<DomainFilterOptions<TDomainService>>>().Get(t.Name).Filters);
                            return filters;
                        });
                return _Filters[_executionContext!.DomainMethod!.Name];
            }

            protected DomainExecutionPipeline MakePipeline()
            {
                var globalFilters = GetFilters();
                DomainExecutionPipeline pipeline = async () =>
                {
                    var logger = DomainContext.GetRequiredService<ILogger<TDomainService>>();
                    using (logger.BeginScope(new DomainServiceInvokerLogState(typeof(TDomainService), _executionContext.DomainMethod)))
                        await ExecuteAsync();
                };
                var filters = DomainContext.Filters;
                for (int i = filters.Count - 1; i >= 0; i--)
                {
                    var next = pipeline;
                    var index = i;
                    pipeline = () =>
                    {
                        if (_executionContext!.IsCompleted)
                            return Task.CompletedTask;
                        return filters[index].OnExecutionAsync(_executionContext!, next);
                    };
                }
                for (int i = globalFilters.Count - 1; i >= 0; i--)
                {
                    var next = pipeline;
                    var index = i;
                    pipeline = () =>
                    {
                        if (_executionContext!.IsCompleted)
                            return Task.CompletedTask;
                        return globalFilters[index].OnExecutionAsync(_executionContext!, next);
                    };
                }
                return pipeline;
            }

            protected void HandleResult(Task task)
            {
                if (task.IsFaulted)
                    System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(task.Exception).Throw();
                _executionContext!.Done();
            }

            protected Task RunPipeline()
            {
                _executionContext = DomainService.Context;
                return MakePipeline()();
            }
        }

        public abstract class DomainServiceInvoker<TDomainService, TResult> : DomainServiceInvoker<TDomainService>
            where TDomainService : class, IDomainService
        {
            protected DomainServiceInvoker(TDomainService domainService, IDomainContext domainContext) : base(domainService, domainContext)
            {

            }

            private TResult? _result;
            private bool _executed;

            protected void HandleResult(Task<TResult> task)
            {
                if (task.IsFaulted)
                    System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(task.Exception).Throw();
                _result = task.Result;
                _executed = true;
                _executionContext!.Done(_result);
            }

            protected TResult? ReturnResult(Task task)
            {
                if (task.IsFaulted)
                    System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(task.Exception).Throw();
                if (_executed)
                    return _result;
                if (_executionContext!.Result == null && typeof(TResult).IsValueType)
                    return default;
                return (TResult?)_executionContext!.Result;
            }
        }
    }

    public class DomainServiceInvokerBuilder<TDomainService> : DomainServiceInvokerBuilder
        where TDomainService : class, IDomainService
    {
        private static Dictionary<MethodInfo, DomainServiceInvokerReference> _References = new Dictionary<MethodInfo, DomainServiceInvokerReference>();

        static DomainServiceInvokerBuilder()
        {
            foreach (var method in typeof(TDomainService).GetMethods(BindingFlags.Public | BindingFlags.Instance).Where(t => !t.IsSpecialName))
            {
                if (method.ReturnType == typeof(Task) || (method.ReturnType.IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>)))
                    _References.Add(method, CreateInvoker(method));
            }
        }

        private static DomainServiceInvokerReference CreateInvoker(MethodInfo method)
        {
            Type baseType;
            if (method.ReturnType == typeof(Task))
                baseType = typeof(DomainServiceInvoker<TDomainService>);
            else
                baseType = typeof(DomainServiceInvoker<,>).MakeGenericType(typeof(TDomainService), method.ReturnType.GetGenericArguments()[0]);
            var ns = "Wodsoft.ComBoost.Dynamic." + typeof(TDomainService).Name + "_" + typeof(TDomainService).GetHashCode() + ".";
            var type = DomainTemplateBuilder.Module.DefineType(ns + method.Name + "Invoker", TypeAttributes.Public | TypeAttributes.Class, baseType);

            var constructor = type.DefineConstructor(MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, CallingConventions.Standard, new Type[] { typeof(TDomainService), typeof(IDomainContext) });
            var constructorILGenerator = constructor.GetILGenerator();
            //:base(TDomainService,IDomainContext);
            constructorILGenerator.Emit(OpCodes.Ldarg_0);
            constructorILGenerator.Emit(OpCodes.Ldarg_1);
            constructorILGenerator.Emit(OpCodes.Ldarg_2);
            constructorILGenerator.Emit(OpCodes.Call, baseType.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)[0]);
            constructorILGenerator.Emit(OpCodes.Ret);

            var executeMethod = type.DefineMethod("ExecuteAsync", MethodAttributes.Family | MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.Final, typeof(Task), null);
            var executeILGenerator = executeMethod.GetILGenerator();
            //Service.{method}()
            executeILGenerator.Emit(OpCodes.Ldarg_0);
            executeILGenerator.Emit(OpCodes.Call, baseType.GetProperty("DomainService").GetGetMethod());
            type.DefineMethodOverride(executeMethod, baseType.GetMethod("ExecuteAsync", BindingFlags.NonPublic | BindingFlags.Instance));

            var invokeMethod = type.DefineMethod("InvokeAsync", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot, method.ReturnType, method.GetParameters().Select(t => t.ParameterType).ToArray());
            var invokeILGenerator = invokeMethod.GetILGenerator();

            int count = 0;
            var parameters = method.GetParameters();
            foreach (var parameter in parameters)
            {
                var field = type.DefineField("Parameter" + parameter.Name, parameter.ParameterType, FieldAttributes.Public);
                //Load value for calling method
                executeILGenerator.Emit(OpCodes.Ldarg_0);
                executeILGenerator.Emit(OpCodes.Ldfld, field);
                //Save value from InvokeAsync parameters
                invokeILGenerator.Emit(OpCodes.Ldarg_0);
                invokeILGenerator.Emit(OpCodes.Ldarg_S, (byte)(count + 1));
                invokeILGenerator.Emit(OpCodes.Stfld, field);
                count++;
            }

            //Call IDomainService.Initialize
            invokeILGenerator.Emit(OpCodes.Ldarg_0);
            invokeILGenerator.Emit(OpCodes.Call, typeof(DomainServiceInvoker<TDomainService>).GetProperty("DomainService").GetGetMethod());
            //Create DomainExecutionContext
            //DomainService
            invokeILGenerator.Emit(OpCodes.Dup);
            //DomainContext
            invokeILGenerator.Emit(OpCodes.Ldarg_0);
            invokeILGenerator.Emit(OpCodes.Call, typeof(DomainServiceInvoker<TDomainService>).GetProperty("DomainContext").GetGetMethod());
            //MethodInfo
            invokeILGenerator.Emit(OpCodes.Ldtoken, method);
            invokeILGenerator.Emit(OpCodes.Ldtoken, method.DeclaringType);
            invokeILGenerator.Emit(OpCodes.Call, _GetMethodMethod);

            invokeILGenerator.Emit(OpCodes.Ldc_I4, count);
            invokeILGenerator.Emit(OpCodes.Newarr, typeof(object));
            for (int i = 0; i < count; i++)
            {
                invokeILGenerator.Emit(OpCodes.Dup);
                invokeILGenerator.Emit(OpCodes.Ldc_I4, i);
                invokeILGenerator.Emit(OpCodes.Ldarg_S, (byte)(i + 1));
                if (parameters[i].ParameterType.IsValueType)
                    invokeILGenerator.Emit(OpCodes.Box, parameters[i].ParameterType);
                invokeILGenerator.Emit(OpCodes.Stelem_Ref);
            }
            invokeILGenerator.Emit(OpCodes.Newobj, _ExecutionContextConstructor);
            invokeILGenerator.Emit(OpCodes.Callvirt, _Initialize);

            invokeILGenerator.Emit(OpCodes.Ldarg_0);
            invokeILGenerator.Emit(OpCodes.Call, baseType.GetMethod("RunPipeline", BindingFlags.NonPublic | BindingFlags.Instance));

            executeILGenerator.Emit(OpCodes.Callvirt, method);
            if (method.ReturnType != typeof(Task))
            {
                invokeILGenerator.Emit(OpCodes.Ldarg_0);
                invokeILGenerator.Emit(OpCodes.Ldftn, baseType.GetMethod("ReturnResult", BindingFlags.NonPublic | BindingFlags.Instance));
                invokeILGenerator.Emit(OpCodes.Newobj, typeof(Func<,>).MakeGenericType(typeof(Task), method.ReturnType.GenericTypeArguments[0]).GetConstructors()[0]);
                invokeILGenerator.Emit(OpCodes.Call, typeof(Task).GetMethods(BindingFlags.Public | BindingFlags.Instance).Where(t => t.Name == "ContinueWith" && t.IsGenericMethodDefinition && t.GetParameters().Length == 1).First().MakeGenericMethod(method.ReturnType.GenericTypeArguments[0]));
            }
            executeILGenerator.Emit(OpCodes.Ldarg_0);
            executeILGenerator.Emit(OpCodes.Ldftn, baseType.GetMethod("HandleResult", BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { method.ReturnType }, null));
            executeILGenerator.Emit(OpCodes.Newobj, typeof(Action<>).MakeGenericType(method.ReturnType).GetConstructors()[0]);
            executeILGenerator.Emit(OpCodes.Call, method.ReturnType.GetMethod("ContinueWith", new Type[] { typeof(Action<>).MakeGenericType(method.ReturnType) }));

            executeILGenerator.Emit(OpCodes.Ret);
            invokeILGenerator.Emit(OpCodes.Ret);

            var invoker = new DomainServiceInvokerReference(type.CreateTypeInfo().AsType());
            return invoker;
        }

        public static DomainServiceInvokerReference GetInvokerReference(MethodInfo method)
        {
            _References.TryGetValue(method, out var invoker);
            return invoker;
        }
    }

    public sealed class DomainServiceInvokerReference
    {
        internal DomainServiceInvokerReference(Type type)
        {
            Type = type;
            InvokeMethod = type.GetMethod("InvokeAsync");
            Constructor = type.GetConstructors()[0];
        }

        public Type Type { get; }

        public MethodInfo InvokeMethod { get; }

        public ConstructorInfo Constructor { get; }
    }
}
