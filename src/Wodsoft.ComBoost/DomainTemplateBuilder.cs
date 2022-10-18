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
        }
    }

    public class DomainTemplateBuilder<TDomainService, T> : DomainTemplateBuilder, IDomainTemplateDescriptor<T>
        where TDomainService : class, IDomainService
        where T : class, IDomainTemplate
    {
        private static Type _TemplateType;

        internal static readonly MethodInfo _GetAttribute = typeof(DomainTemplateBuilder<TDomainService, T>).GetMethod(nameof(GetAttribute), BindingFlags.Public | BindingFlags.Static);
        internal static readonly MethodInfo _GetParameterInfo = typeof(DomainTemplateBuilder<TDomainService, T>).GetMethod("GetParameterInfo");
        internal static readonly MethodInfo _GetDomainService = typeof(DomainTemplateAgent<TDomainService>).GetProperty("Service").GetGetMethod();
        internal static readonly ConstructorInfo _AgentConstructor = typeof(DomainTemplateAgent<TDomainService>).GetConstructor(new Type[] { typeof(IDomainContext), typeof(TDomainService) });
        private static Dictionary<string, FromAttribute[]> _FromAttributes;
        private static Dictionary<string, ParameterInfo[]> _ParameterInfos;
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

        static DomainTemplateBuilder()
        {
            var serviceType = typeof(TDomainService);
            var type = typeof(T);
            _FromAttributes = serviceType.GetMethods().ToDictionary(x => x.Name,
                    x => x.GetParameters().Select(y => (FromAttribute)y.GetCustomAttributes().FirstOrDefault(z => z is FromAttribute)).ToArray());
            _ParameterInfos = serviceType.GetMethods().ToDictionary(x => x.Name, x => x.GetParameters());

            if (!type.IsInterface)
                throw new NotSupportedException("Not support non interface type as a template.");
            if (type.GetFields().Length != 0)
                throw new NotSupportedException("Interface template could not contains fields.");
            if (type.GetProperties().Length != 0)
                throw new NotSupportedException("Interface template could not contains properties.");
            if (type.GetEvents().Length != 0)
                throw new NotSupportedException("Interface template could not contains events.");
            _AgentBuilder = Module.DefineType("Proxy_" + typeof(T).Name + "_" + typeof(T).GetHashCode(), TypeAttributes.Public | TypeAttributes.Class, typeof(DomainTemplateAgent<TDomainService>), new Type[] { type, typeof(IDomainTemplate) });

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
                var serviceMethod = serviceType.GetMethod(method.Name, BindingFlags.Public | BindingFlags.Instance);
                if (serviceMethod == null)
                    throw new NotSupportedException($"Domain service \"{serviceType.FullName}\" does not implement method \"{method}\".");
                if (serviceMethod.ReturnType != method.ReturnType)
                    throw new NotSupportedException($"Return type of {method.DeclaringType.FullName}.{method.Name}({string.Join(",", method.GetParameters().Select(t => t.ParameterType.Name))}) does not equal {serviceMethod.DeclaringType.FullName}.{serviceMethod.Name}({string.Join(",", serviceMethod.GetParameters().Select(t => t.ParameterType.Name))})");
                if (!typeof(Task).IsAssignableFrom(serviceMethod.ReturnType))
                    throw new NotSupportedException($"Return type of {method.DeclaringType.FullName}.{method.Name} must be a Task.");

                var parameters = method.GetParameters();
                //Create method for class
                var methodBuilder = _AgentBuilder.DefineMethod(method.Name,
                    MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual,
                    method.ReturnType, parameters.Select(t => t.ParameterType).ToArray());
                //Define parameters
                for (int i = 0; i < parameters.Length; i++)
                {
                    var parameterBuilder = methodBuilder.DefineParameter(i + 1, parameters[i].Attributes, parameters[i].Name);
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

                //new Invoker(DomainService, DomainContext).InvokeAsync(...)
                ilGenerator.Emit(OpCodes.Ldarg_0);
                ilGenerator.Emit(OpCodes.Call, _GetDomainService);
                ilGenerator.Emit(OpCodes.Ldarg_0);
                ilGenerator.Emit(OpCodes.Call, _GetContext);
                var invoker = DomainServiceInvokerBuilder<TDomainService>.GetInvokerReference(serviceMethod);
                ilGenerator.Emit(OpCodes.Newobj, invoker.Constructor);
                for (int i = 0; i < count; i++)
                    ilGenerator.Emit(OpCodes.Ldloc, locals[i]);
                //Call invoker method and return
                ilGenerator.Emit(OpCodes.Call, invoker.InvokeMethod);
                //ilGenerator.Emit(OpCodes.Ldnull);
                ilGenerator.Emit(OpCodes.Ret);
            }

            var typeInfo = _AgentBuilder.CreateTypeInfo();
            _TemplateType = typeInfo.AsType();
        }

        public T GetTemplate(IDomainContext context)
        {
            var service = context.GetRequiredService<TDomainService>();
            return (T)Activator.CreateInstance(_TemplateType, context, service);
        }
    }
}
