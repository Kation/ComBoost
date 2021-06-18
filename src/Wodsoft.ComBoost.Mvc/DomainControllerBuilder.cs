using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Mvc
{
    public class DomainControllerBuilder
    {
        internal static readonly MethodInfo _GetService = typeof(IServiceProvider).GetMethod("GetService");
        internal static readonly MethodInfo _GetValue = typeof(IValueProvider).GetMethod("GetValue");
        internal static readonly MethodInfo _GetFromValue = typeof(FromAttribute).GetMethod("GetValue");

        public static ModuleBuilder Module { get; }

        static DomainControllerBuilder()
        {
            AssemblyBuilder assembly = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("Wodsoft.ComBoost.Mvc.DynamicAssembly"), AssemblyBuilderAccess.Run);
            Module = assembly.DefineDynamicModule("default");
        }
    }

    public class DomainControllerBuilder<TDomainController, TDomainService> : DomainControllerBuilder
        where TDomainController : DomainController<TDomainService>
        where TDomainService : class, IDomainService
    {
        internal static readonly MethodInfo _GetAttribute = typeof(DomainControllerBuilder<TDomainController, TDomainService>).GetMethod(nameof(GetAttribute), BindingFlags.Public | BindingFlags.Static);
        internal static readonly MethodInfo _GetParameterInfo = typeof(DomainControllerBuilder<TDomainController, TDomainService>).GetMethod("GetParameterInfo");
        internal static readonly MethodInfo _GetDomainService = typeof(DomainController<TDomainService>).GetMethod("GetDomainService", BindingFlags.NonPublic | BindingFlags.Instance);
        internal static readonly MethodInfo _GetDomainContext = typeof(DomainController<TDomainService>).GetMethod("GetDomainContext", BindingFlags.NonPublic | BindingFlags.Instance);
        private static Dictionary<string, FromAttribute[]> _FromAttributes;
        private static Dictionary<string, ParameterInfo[]> _ParameterInfos;

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

        static DomainControllerBuilder()
        {
            var controllerType = typeof(TDomainController);
            var serviceType = typeof(TDomainService);
            _FromAttributes = serviceType.GetMethods().ToDictionary(x => x.Name,
                    x => x.GetParameters().Select(y => (FromAttribute)y.GetCustomAttributes().FirstOrDefault(z => z is FromAttribute)).ToArray());
            _ParameterInfos = serviceType.GetMethods().ToDictionary(x => x.Name, x => x.GetParameters());
            var typeBuilder = Module.DefineType("Wodsoft.ComBoost.Mvc.DynamicAssembly.Controllers." + controllerType.FullName, TypeAttributes.Public | TypeAttributes.Class, controllerType);
            typeBuilder.SetCustomAttribute(new CustomAttributeBuilder(typeof(ControllerAttribute).GetConstructor(Array.Empty<Type>()), Array.Empty<object>()));
            foreach (var serviceMethod in serviceType.GetMethods().Where(method => !method.IsSpecialName && typeof(Task).IsAssignableFrom(method.ReturnType) && !method.IsGenericMethodDefinition))
            {
                var actionBuilder = typeBuilder.DefineMethod(serviceMethod.Name, MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot, serviceMethod.ReturnType, null);
                var ilGenerator = actionBuilder.GetILGenerator();
                var serviceLocal = ilGenerator.DeclareLocal(serviceType);
                var contextLocal = ilGenerator.DeclareLocal(typeof(IDomainContext));
                var valueProviderLocal = ilGenerator.DeclareLocal(typeof(IValueProvider));

                ilGenerator.Emit(OpCodes.Ldarg_0);
                ilGenerator.Emit(OpCodes.Callvirt, _GetDomainService);
                ilGenerator.Emit(OpCodes.Stloc, serviceLocal);

                ilGenerator.Emit(OpCodes.Ldarg_0);
                ilGenerator.Emit(OpCodes.Callvirt, _GetDomainContext);
                ilGenerator.Emit(OpCodes.Dup);
                ilGenerator.Emit(OpCodes.Stloc, contextLocal);

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

                        ilGenerator.Emit(OpCodes.Ldloc, contextLocal);

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
                    count++;
                }

                //Call invoker method and return
                ilGenerator.Emit(OpCodes.Ldloc, serviceLocal);
                ilGenerator.Emit(OpCodes.Ldloc, contextLocal);
                var invoker = DomainServiceInvokerBuilder<TDomainService>.GetInvokerReference(serviceMethod);
                ilGenerator.Emit(OpCodes.Newobj, invoker.Constructor);
                for (int i = 0; i < count; i++)
                    ilGenerator.Emit(OpCodes.Ldloc, locals[i]);
                ilGenerator.Emit(OpCodes.Call, invoker.InvokeMethod);
                ilGenerator.Emit(OpCodes.Ret);
            }
            ControllerType = typeBuilder.CreateType();
        }

        public static Type ControllerType { get; }
    }
}
