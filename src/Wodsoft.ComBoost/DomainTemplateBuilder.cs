using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    public class DomainTemplateBuilder
    {
        internal static readonly ConstructorInfo _DictionaryConstructor = typeof(Dictionary<string, object>).GetConstructor(new Type[0]);
        internal static readonly MethodInfo _DictionaryAdd = typeof(Dictionary<string, object>).GetMethod("Add", new Type[] { typeof(string), typeof(object) });
        internal static readonly MethodInfo _ExecuteAsync = typeof(IDomainService).GetMethod("ExecuteAsync");
        internal static readonly MethodInfo _ExecuteWithValueAsync = typeof(IDomainService).GetMethod("ExecuteWithValueAsync");
        internal static readonly MethodInfo _SetValues = typeof(DomainTemplateAgent).GetMethod("SetValues", BindingFlags.NonPublic | BindingFlags.Instance);
        internal static readonly MethodInfo _GetFromValue = typeof(FromAttribute).GetMethod("GetValue");
        internal static readonly MethodInfo _GetContext = typeof(DomainTemplateAgent).GetProperty("Context").GetGetMethod();
        internal static readonly MethodInfo _GetMethodMethod = typeof(MethodBase).GetMethod("GetMethodFromHandle", BindingFlags.Public | BindingFlags.Static, null, new[] { typeof(RuntimeMethodHandle) }, null);
        internal static readonly MethodInfo _GetService = typeof(IServiceProvider).GetMethod("GetService");
        internal static readonly MethodInfo _GetValue = typeof(IValueProvider).GetMethod("GetValue");
        internal static readonly MethodInfo _Initialize = typeof(IDomainService).GetMethod("Initialize");
        internal static readonly ConstructorInfo _ExecutionContextConstructor = typeof(DomainExecutionContext).GetConstructor(new Type[] { typeof(IDomainService), typeof(IDomainContext), typeof(MethodInfo), typeof(object[]) });


        public static ModuleBuilder Module { get; }

        static DomainTemplateBuilder()
        {
            AssemblyBuilder assembly = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("Wodsoft.ComBoost.DynamicAssembly"), AssemblyBuilderAccess.Run);
            Module = assembly.DefineDynamicModule("default");
        }

        public abstract class DomainTemplateAgent : IDomainTemplate
        {
            public DomainTemplateAgent(IDomainContext context)
            {
                _context = context;
            }

            private IDomainContext _context;
            public IDomainContext Context { get => _context; set => _context = value ?? throw new ArgumentNullException(nameof(value)); }

            protected void SetValues(Dictionary<string, object> values)
            {
                var valueProvider = Context.GetRequiredService<IConfigurableValueProvider>();
                foreach (var value in values)
                    valueProvider.SetValue(value.Key, value.Value);
            }
        }

        public abstract class DomainTemplateAgent<TService> : DomainTemplateAgent
            where TService : class, IDomainService
        {
            public DomainTemplateAgent(IDomainContext context, TService service) : base(context)
            {
                Service = service;
            }

            public TService Service { get; }

            protected IList<IDomainServiceFilter> GetFilters(string methodName)
            {
                List<IDomainServiceFilter> items = new List<IDomainServiceFilter>();
                items.AddRange(Context.GetService<IOptions<DomainFilterOptions>>().Value.Filters);
                var serviceFilterOptions = Context.GetService<IOptionsMonitor<DomainFilterOptions<TService>>>();
                items.AddRange(serviceFilterOptions.CurrentValue.Filters);
                items.AddRange(serviceFilterOptions.Get(methodName).Filters);
                return items;
            }
        }

        public abstract class DomainTemplateInvoker<TDomainService>
            where TDomainService : class, IDomainService
        {
            protected DomainTemplateInvoker(TDomainService domainService)
            {
                DomainService = domainService;
            }

            public TDomainService DomainService { get; }

            private static List<IDomainServiceFilter> _Filters;

            protected abstract Task ExecuteAsync();

            protected IList<IDomainServiceFilter> GetFilters()
            {
                if (_Filters == null)
                {
                    List<IDomainServiceFilter> filters = new List<IDomainServiceFilter>();
                    filters.AddRange(DomainService.Context.DomainContext.GetService<IOptions<DomainFilterOptions>>().Value.Filters);
                    filters.AddRange(typeof(TDomainService).GetCustomAttributes<DomainServiceFilterAttribute>());
                    filters.AddRange(DomainService.Context.DomainContext.GetService<IOptions<DomainFilterOptions<TDomainService>>>().Value.Filters);
                    filters.AddRange(DomainService.Context.DomainMethod.GetCustomAttributes<DomainServiceFilterAttribute>());
                    filters.AddRange(DomainService.Context.DomainContext.GetService<IOptionsMonitor<DomainFilterOptions<TDomainService>>>().Get(DomainService.Context.DomainMethod.Name).Filters);
                    _Filters = filters;
                }
                return _Filters;
            }

            protected DomainExecutionPipeline MakePipeline()
            {
                var globalFilters = GetFilters();
                DomainExecutionPipeline pipeline = ExecuteAsync;
                var filters = DomainService.Context.DomainContext.Filter;
                var context = DomainService.Context;
                for (int i = filters.Count - 1; i >= 0; i--)
                {
                    var next = pipeline;
                    var index = i;
                    pipeline = () => filters[index].OnExecutionAsync(context, next);
                }
                for (int i = globalFilters.Count - 1; i >= 0; i--)
                {
                    var next = pipeline;
                    var index = i;
                    pipeline = () => globalFilters[index].OnExecutionAsync(context, next);
                }
                return pipeline;
            }

            protected void HandleResult(Task task)
            {
                if (!task.IsCompleted)
                    System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(task.Exception).Throw();
                DomainService.Context.Done();
            }

            protected Task RunPipeline()
            {
                return MakePipeline()();
            }
        }

        public abstract class DomainTemplateInvoker<TDomainService, TResult> : DomainTemplateInvoker<TDomainService>
            where TDomainService : class, IDomainService
        {
            protected DomainTemplateInvoker(TDomainService domainService) : base(domainService)
            {

            }

            private TResult result;

            protected void HandleResult(Task<TResult> task)
            {
                if (task.IsFaulted)
                    System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(task.Exception).Throw();
                result = task.Result;
                DomainService.Context.Done(result);
            }

            protected TResult ReturnResult(Task task)
            {
                if (task.IsFaulted)
                    System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(task.Exception).Throw();
                return result;
            }
        }
    }

    public class DomainTemplateBuilder<TDomainService, T> : DomainTemplateBuilder, IDomainTemplateDescriptor<T>
        where TDomainService : class, IDomainService
        where T : class, IDomainTemplate
    {
        private static Type _TemplateType;

        internal static readonly MethodInfo _GetAttribute = typeof(DomainTemplateBuilder<TDomainService, T>).GetMethod(nameof(GetAttribute), BindingFlags.Public | BindingFlags.Static);
        internal static readonly MethodInfo _GetParameterInfo = typeof(DomainTemplateBuilder<TDomainService, T>).GetMethod("GetParameterInfo");
        internal static readonly MethodInfo _GetMethodInfo = typeof(DomainTemplateBuilder<TDomainService, T>).GetMethod("GetMethodInfo");
        internal static readonly MethodInfo _GetDomainService = typeof(DomainTemplateAgent<TDomainService>).GetProperty("Service").GetGetMethod();
        internal static readonly ConstructorInfo _AgentConstructor = typeof(DomainTemplateAgent<TDomainService>).GetConstructor(new Type[] { typeof(IDomainContext), typeof(TDomainService) });
        private static Dictionary<string, FromAttribute[]> _FromAttributes;
        private static Dictionary<string, ParameterInfo[]> _ParameterInfos;
        private static Dictionary<string, MethodInfo> _MethodInfos;
        private static Dictionary<string, Invoker> _Invoker = new Dictionary<string, Invoker>();
        private static TypeBuilder _AgentBuilder;

        public static FromAttribute GetAttribute(string method, int parameterIndex)
        {
            var values = _FromAttributes[method];
            return values[parameterIndex];
        }

        public static ParameterInfo GetParameterInfo(string method, int parameterIndex)
        {
            var values = _ParameterInfos[method];
            return values[parameterIndex];
        }

        public static MethodInfo GetMethodInfo(string method)
        {
            return _MethodInfos[method];
        }

        static DomainTemplateBuilder()
        {
            var serviceType = typeof(TDomainService);
            var type = typeof(T);
            _FromAttributes = serviceType.GetMethods().ToDictionary(x => x.Name,
                    x => x.GetParameters().Select(y => (FromAttribute)y.GetCustomAttributes().FirstOrDefault(z => z is FromAttribute)).ToArray());
            _ParameterInfos = serviceType.GetMethods().ToDictionary(x => x.Name, x => x.GetParameters());
            _MethodInfos = serviceType.GetMethods().ToDictionary(t => t.Name, t => t);

            if (!type.IsInterface)
                throw new NotSupportedException("Not support non interface type as a template.");
            if (type.GetFields().Length != 0)
                throw new NotSupportedException("Interface template could not contains fields.");
            if (type.GetProperties().Length != 0)
                throw new NotSupportedException("Interface template could not contains properties.");
            if (type.GetEvents().Length != 0)
                throw new NotSupportedException("Interface template could not contains events.");
            _AgentBuilder = Module.DefineType("Proxy_" + typeof(T).Name, TypeAttributes.Public | TypeAttributes.Class, typeof(DomainTemplateAgent<TDomainService>), new Type[] { type, typeof(IDomainTemplate) });

            //Create constructor ctor(IDomainContext, IDomainService)
            {
                var constructor = _AgentBuilder.DefineConstructor(MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, CallingConventions.Standard,
                    new Type[] { typeof(IDomainContext), typeof(TDomainService) });
                var ilGenerator = constructor.GetILGenerator();
                ilGenerator.Emit(OpCodes.Ldarg_0);
                ilGenerator.Emit(OpCodes.Ldarg_1);
                ilGenerator.Emit(OpCodes.Ldarg_2);
                ilGenerator.Emit(OpCodes.Call, _AgentConstructor);
                ilGenerator.Emit(OpCodes.Ret);
            }

            List<MethodInfo> methods = new List<MethodInfo>();
            methods.AddRange(type.GetMethods());
            Queue<Type> interfaces = new Queue<Type>(type.GetInterfaces());
            while (interfaces.Count > 0)
            {
                var item = interfaces.Dequeue();
                if (item == typeof(IDomainTemplate))
                    continue;
                methods.AddRange(item.GetMethods());
                foreach (var i in item.GetInterfaces())
                    interfaces.Enqueue(i);
            }

            //Implement methods in interface
            foreach (var method in methods)
            {
                //Get service method what template invoke to
                var serviceMethod = serviceType.GetMethod(method.Name);

                if (serviceMethod.ReturnType != method.ReturnType)
                    throw new NotSupportedException($"Return type of {method.DeclaringType.FullName}.{method.Name}({string.Join(",", method.GetParameters().Select(t => t.ParameterType.Name))}) does not equal {serviceMethod.DeclaringType.FullName}.{serviceMethod.Name}({string.Join(",", serviceMethod.GetParameters().Select(t => t.ParameterType.Name))})");

                //Create method for class
                var methodBuilder = _AgentBuilder.DefineMethod(method.Name,
                    MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual,
                    method.ReturnType, method.GetParameters().Select(t => t.ParameterType).ToArray());
                var parameters = method.GetParameters();
                //Define parameters
                for (int i = 0; i < parameters.Length; i++)
                {
                    var parameterBuilder = methodBuilder.DefineParameter(i, parameters[i].Attributes, parameters[i].Name);
                    if (parameters[i].HasDefaultValue)
                        parameterBuilder.SetConstant(parameters[i].DefaultValue);
                }
                //Build il codes
                var ilGenerator = methodBuilder.GetILGenerator();

                //var values = new Dictionary<string, object>();
                var values = ilGenerator.DeclareLocal(typeof(Dictionary<string, object>));
                ilGenerator.Emit(OpCodes.Newobj, _DictionaryConstructor);
                ilGenerator.Emit(OpCodes.Stloc, values);
                //Set each parameter value into values. values.Add(parameter name, parameter value)
                for (int i = 0; i < parameters.Length; i++)
                {
                    ilGenerator.Emit(OpCodes.Ldloc, values);
                    ilGenerator.Emit(OpCodes.Ldstr, parameters[i].Name);
                    ilGenerator.Emit(OpCodes.Ldarg_S, (byte)(i + 1));
                    if (parameters[i].ParameterType.IsValueType)
                        ilGenerator.Emit(OpCodes.Box, parameters[i].ParameterType);
                    ilGenerator.Emit(OpCodes.Call, _DictionaryAdd);
                }

                //SetValues(values);
                ilGenerator.Emit(OpCodes.Ldarg_0);
                ilGenerator.Emit(OpCodes.Ldloc, values);
                ilGenerator.Emit(OpCodes.Call, _SetValues);

                var valueProviderLocal = ilGenerator.DeclareLocal(typeof(IValueProvider));

                //var valueProvider = (IValueProvider)this.Context.GetService(typeof(IValueProvider));
                ilGenerator.Emit(OpCodes.Ldarg_0);
                ilGenerator.Emit(OpCodes.Call, _GetContext);
                ilGenerator.Emit(OpCodes.Ldtoken, typeof(IValueProvider));
                ilGenerator.Emit(OpCodes.Call, typeof(Type).GetMethod("GetTypeFromHandle", new Type[1] { typeof(RuntimeTypeHandle) }));
                ilGenerator.Emit(OpCodes.Callvirt, _GetService);
                ilGenerator.Emit(OpCodes.Castclass, typeof(IValueProvider));
                ilGenerator.Emit(OpCodes.Stloc, valueProviderLocal);

                int count = 0;
                LocalBuilder[] locals = new LocalBuilder[serviceMethod.GetParameters().Length];
                //Prepare domain method parameters
                foreach (var serviceParameter in serviceMethod.GetParameters())
                {
                    locals[count] = ilGenerator.DeclareLocal(serviceParameter.ParameterType);
                    var methodParameter = parameters.FirstOrDefault(t => t.Name == serviceParameter.Name);
                    if (methodParameter == null)
                    {
                        //Non template value, get value by FromAttribute

                        var fromAttribute = serviceParameter.GetCustomAttributes().Where(t => t is FromAttribute).Cast<FromAttribute>().FirstOrDefault();
                        if (fromAttribute == null)
                        {
                            //When there is no FromAtrribute, get value from IValueProvider
                            ilGenerator.Emit(OpCodes.Ldloc, valueProviderLocal);
                            ilGenerator.Emit(OpCodes.Ldstr, serviceParameter.Name);
                            ilGenerator.Emit(OpCodes.Ldtoken, serviceParameter.ParameterType);
                            ilGenerator.Emit(OpCodes.Call, typeof(Type).GetMethod("GetTypeFromHandle", new Type[1] { typeof(RuntimeTypeHandle) }));
                            ilGenerator.Emit(OpCodes.Callvirt, _GetValue);
                            if (serviceParameter.ParameterType.IsValueType)
                                ilGenerator.Emit(OpCodes.Unbox_Any, serviceParameter.ParameterType);
                            else
                                ilGenerator.Emit(OpCodes.Castclass, serviceParameter.ParameterType);
                            ilGenerator.Emit(OpCodes.Stloc, locals[count]);
                        }
                        else
                        {
                            //GetAttribute({serviceMethod}, {count});
                            ilGenerator.Emit(OpCodes.Ldstr, serviceMethod.Name);
                            ilGenerator.Emit(OpCodes.Ldc_I4, count);
                            ilGenerator.Emit(OpCodes.Call, _GetAttribute);

                            ilGenerator.Emit(OpCodes.Ldarg_0);
                            ilGenerator.Emit(OpCodes.Call, _GetContext);

                            ilGenerator.Emit(OpCodes.Ldstr, serviceMethod.Name);
                            ilGenerator.Emit(OpCodes.Ldc_I4, count);
                            ilGenerator.Emit(OpCodes.Call, _GetParameterInfo);

                            ilGenerator.Emit(OpCodes.Callvirt, _GetFromValue);
                            if (serviceParameter.ParameterType.IsValueType)
                                ilGenerator.Emit(OpCodes.Unbox_Any, serviceParameter.ParameterType);
                            else
                                ilGenerator.Emit(OpCodes.Castclass, serviceParameter.ParameterType);
                            ilGenerator.Emit(OpCodes.Stloc, locals[count]);
                        }
                    }
                    else
                    {
                        if (methodParameter.ParameterType != serviceParameter.ParameterType)
                            throw new NotSupportedException($"Type of parameter \"{methodParameter.Name}\" of {methodParameter.Member.DeclaringType.FullName}.{methodParameter.Member.Name}({string.Join(",", ((MethodInfo)methodParameter.Member).GetParameters().Select(t => t.ParameterType.Name))}) does not equal to {serviceParameter.Member.DeclaringType.FullName}.{serviceParameter.Member.Name}({string.Join(",", ((MethodInfo)serviceParameter.Member).GetParameters().Select(t => t.ParameterType.Name))})");
                        var methodParameterIndex = Array.IndexOf(parameters, methodParameter);
                        ilGenerator.Emit(OpCodes.Ldarg_S, (byte)(methodParameterIndex + 1));
                        ilGenerator.Emit(OpCodes.Stloc, locals[count]);
                    }
                    count++;
                }

                //Call IDomainService.Initialize
                ilGenerator.Emit(OpCodes.Ldarg_0);
                ilGenerator.Emit(OpCodes.Call, _GetDomainService);
                //Create DomainExecutionContext
                //Copy for IDomainService parameter
                ilGenerator.Emit(OpCodes.Dup);
                //Copy for create DomainTemplateInvoker
                ilGenerator.Emit(OpCodes.Dup);
                //IDomainContext
                ilGenerator.Emit(OpCodes.Ldarg_0);
                ilGenerator.Emit(OpCodes.Call, _GetContext);
                //MethodInfo
                ilGenerator.Emit(OpCodes.Ldstr, serviceMethod.Name);
                ilGenerator.Emit(OpCodes.Call, _GetMethodInfo);
                //Parameters
                ilGenerator.Emit(OpCodes.Ldc_I4, count);
                ilGenerator.Emit(OpCodes.Newarr, typeof(object));
                for (int i = 0; i < count; i++)
                {
                    ilGenerator.Emit(OpCodes.Dup);
                    ilGenerator.Emit(OpCodes.Ldc_I4, i);
                    ilGenerator.Emit(OpCodes.Ldloc, locals[i]);
                    if (locals[i].LocalType.IsValueType)
                        ilGenerator.Emit(OpCodes.Box, locals[i].LocalType);
                    ilGenerator.Emit(OpCodes.Stelem_Ref);
                }
                ilGenerator.Emit(OpCodes.Newobj, _ExecutionContextConstructor);
                ilGenerator.Emit(OpCodes.Callvirt, _Initialize);

                var invoker = GetInvoker(serviceMethod);
                ilGenerator.Emit(OpCodes.Newobj, invoker.Constructor);
                for (int i = 0; i < count; i++)
                    ilGenerator.Emit(OpCodes.Ldloc, locals[i]);

                //Call invoker method and return
                ilGenerator.Emit(OpCodes.Call, invoker.InvokeMethod);

                //ilGenerator.Emit(OpCodes.Ldnull);
                ilGenerator.Emit(OpCodes.Ret);
            }

            var typeInfo = _AgentBuilder.CreateTypeInfo();
            foreach (var invoker in _Invoker)
                invoker.Value.Type.CreateTypeInfo();
            _TemplateType = typeInfo.AsType();
        }

        private static Invoker GetInvoker(MethodInfo method)
        {
            if (!_Invoker.TryGetValue(method.Name, out var invoker))
            {
                Type baseType;
                if (method.ReturnType == typeof(Task))
                    baseType = typeof(DomainTemplateInvoker<TDomainService>);
                else
                    baseType = typeof(DomainTemplateInvoker<,>).MakeGenericType(typeof(TDomainService), method.ReturnType.GetGenericArguments()[0]);
                var type = _AgentBuilder.DefineNestedType(method.Name + "Invoker", TypeAttributes.NestedPublic | TypeAttributes.Class, baseType);

                var constructor = type.DefineConstructor(MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, CallingConventions.Standard, new Type[] { typeof(TDomainService) });
                var constructorILGenerator = constructor.GetILGenerator();
                constructorILGenerator.Emit(OpCodes.Ldarg_0);
                constructorILGenerator.Emit(OpCodes.Ldarg_1);
                constructorILGenerator.Emit(OpCodes.Call, baseType.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)[0]);
                constructorILGenerator.Emit(OpCodes.Ret);

                var executeMethod = type.DefineMethod("ExecuteAsync", MethodAttributes.Family | MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.Final, typeof(Task), null);
                var executeILGenerator = executeMethod.GetILGenerator();
                //Service.{method}
                executeILGenerator.Emit(OpCodes.Ldarg_0);
                executeILGenerator.Emit(OpCodes.Call, baseType.GetProperty("DomainService").GetGetMethod());
                type.DefineMethodOverride(executeMethod, baseType.GetMethod("ExecuteAsync", BindingFlags.NonPublic | BindingFlags.Instance));

                var invokeMethod = type.DefineMethod("InvokeAsync", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot, method.ReturnType, method.GetParameters().Select(t => t.ParameterType).ToArray());
                var invokeILGenerator = invokeMethod.GetILGenerator();

                int count = 0;
                foreach (var parameter in method.GetParameters())
                {
                    var field = type.DefineField("Parameter" + parameter.Name, parameter.ParameterType, FieldAttributes.Public);
                    executeILGenerator.Emit(OpCodes.Ldarg_0);
                    executeILGenerator.Emit(OpCodes.Ldfld, field);
                    invokeILGenerator.Emit(OpCodes.Ldarg_0);
                    invokeILGenerator.Emit(OpCodes.Ldarg_S, (byte)(count + 1));
                    invokeILGenerator.Emit(OpCodes.Stfld, field);
                    count++;
                }

                invokeILGenerator.Emit(OpCodes.Ldarg_0);
                invokeILGenerator.Emit(OpCodes.Call, baseType.GetMethod("RunPipeline", BindingFlags.NonPublic | BindingFlags.Instance));
                //invokeILGenerator.Emit(OpCodes.Ldarg_0);

                executeILGenerator.Emit(OpCodes.Callvirt, method);
                if (method.ReturnType != typeof(Task))
                {
                    invokeILGenerator.Emit(OpCodes.Ldarg_0);
                    invokeILGenerator.Emit(OpCodes.Ldftn, baseType.GetMethod("ReturnResult", BindingFlags.NonPublic | BindingFlags.Instance));
                    invokeILGenerator.Emit(OpCodes.Newobj, typeof(Func<,>).MakeGenericType(typeof(Task), method.ReturnType.GenericTypeArguments[0]).GetConstructors()[0]);
                    //invokeILGenerator.Emit(OpCodes.Ldc_I4, (int)TaskContinuationOptions.OnlyOnRanToCompletion);
                    invokeILGenerator.Emit(OpCodes.Call, typeof(Task).GetMethods(BindingFlags.Public | BindingFlags.Instance).Where(t => t.Name == "ContinueWith" && t.IsGenericMethodDefinition && t.GetParameters().Length == 1/* && t.GetParameters()[1].ParameterType == typeof(TaskContinuationOptions)*/).First().MakeGenericMethod(method.ReturnType.GenericTypeArguments[0]));
                }
                executeILGenerator.Emit(OpCodes.Ldarg_0);
                executeILGenerator.Emit(OpCodes.Ldftn, baseType.GetMethod("HandleResult", BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { method.ReturnType }, null));
                executeILGenerator.Emit(OpCodes.Newobj, typeof(Action<>).MakeGenericType(method.ReturnType).GetConstructors()[0]);
                //executeILGenerator.Emit(OpCodes.Ldc_I4, (int)TaskContinuationOptions.OnlyOnRanToCompletion);
                executeILGenerator.Emit(OpCodes.Call, method.ReturnType.GetMethod("ContinueWith", new Type[] { typeof(Action<>).MakeGenericType(method.ReturnType)/*, typeof(TaskContinuationOptions)*/ }));

                executeILGenerator.Emit(OpCodes.Ret);
                invokeILGenerator.Emit(OpCodes.Ret);

                invoker = new Invoker
                {
                    Constructor = constructor,
                    InvokeMethod = invokeMethod,
                    Type = type
                };
                _Invoker[method.Name] = invoker;
            }
            return invoker;
        }

        public T GetTemplate(IDomainContext context)
        {
            var service = context.GetRequiredService<TDomainService>();
            return (T)Activator.CreateInstance(_TemplateType, context, service);
        }

        private class Invoker
        {
            public TypeBuilder Type;
            public MethodBuilder InvokeMethod;
            public ConstructorBuilder Constructor;
        }
    }
}
