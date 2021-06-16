using Grpc.Core;
using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Wodsoft.Protobuf;

namespace Wodsoft.ComBoost.Grpc.Client
{
    public class GrpcTemplateBuilder
    {
        internal static ModuleBuilder Module { get; }

        static GrpcTemplateBuilder()
        {
            AssemblyBuilder assembly = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("Wodsoft.ComBoost.Grpc.Client.Dynamic"), AssemblyBuilderAccess.Run);
            Module = assembly.DefineDynamicModule("default");
        }

        protected internal static ConstructorInfo _TemplateConstructorInfo = typeof(GrpcTemplate).GetConstructor(new Type[] { typeof(GrpcChannel) });
        protected internal static MethodInfo _InvokeAsyncMethodInfo = typeof(GrpcTemplate).GetMethod("InvokeAsync", BindingFlags.NonPublic | BindingFlags.Instance);
        protected internal static MethodInfo _InvokeWithReturnValueAsyncMethodInfo = typeof(GrpcTemplate).GetMethod("InvokeWithReturnValueAsync", BindingFlags.NonPublic | BindingFlags.Instance);
        protected internal static MethodInfo _InvokeWithArgumentsAsyncMethodInfo = typeof(GrpcTemplate).GetMethod("InvokeWithArgumentsAsync", BindingFlags.NonPublic | BindingFlags.Instance);
        protected internal static MethodInfo _InvokeWithArgumentsAndReturnValueAsyncMethodInfo = typeof(GrpcTemplate).GetMethod("InvokeWithArgumentsAndReturnValueAsync", BindingFlags.NonPublic | BindingFlags.Instance);

        public class GrpcTemplate : IDomainTemplate
        {
            public GrpcTemplate(GrpcChannel channel)
            {
                Channel = channel ?? throw new ArgumentNullException(nameof(channel));
            }

            public IDomainContext Context { get; set; }

            public GrpcChannel Channel { get; }

            protected async Task InvokeAsync(Method<DomainGrpcRequest, DomainGrpcResponse> method)
            {
                DomainGrpcRequest request = new DomainGrpcRequest();
                HandleRequest(request);
                var invoker = Channel.CreateCallInvoker();
                var response = await invoker.AsyncUnaryCall(method, null, default(CallOptions), request);
                HandleResponse(response);
            }

            protected async Task<TValue> InvokeWithReturnValueAsync<TValue>(Method<DomainGrpcRequest, DomainGrpcResponse<TValue>> method)
            {
                DomainGrpcRequest request = new DomainGrpcRequest();
                HandleRequest(request);
                var invoker = Channel.CreateCallInvoker();
                var response = await invoker.AsyncUnaryCall(method, null, default(CallOptions), request);
                HandleResponse(response);
                return response.Result;
            }

            protected async Task InvokeWithArgumentsAsync<TArgs>(Method<DomainGrpcRequest<TArgs>, DomainGrpcResponse> method, TArgs args)
            {
                DomainGrpcRequest<TArgs> request = new DomainGrpcRequest<TArgs>();
                request.Argument = args;
                HandleRequest(request);
                var invoker = Channel.CreateCallInvoker();
                var response = await invoker.AsyncUnaryCall(method, null, default(CallOptions), request);
                HandleResponse(response);
            }

            protected async Task<TValue> InvokeWithArgumentsAndReturnValueAsync<TArgs, TValue>(Method<DomainGrpcRequest<TArgs>, DomainGrpcResponse<TValue>> method, TArgs args)
            {
                DomainGrpcRequest<TArgs> request = new DomainGrpcRequest<TArgs>();
                request.Argument = args;
                HandleRequest(request);
                var invoker = Channel.CreateCallInvoker();
                var response = await invoker.AsyncUnaryCall(method, null, default(CallOptions), request);
                HandleResponse(response);
                return response.Result;
            }

            private void HandleRequest(DomainGrpcRequest request)
            {
                request.OS = Environment.OSVersion.ToString();
            }

            private void HandleResponse(DomainGrpcResponse response)
            {
                if (response.Exception != null)
                    throw new DomainGrpcInvokeException(response.Exception);
            }
        }
    }

    public class GrpcTemplateBuilder<T> : GrpcTemplateBuilder, IDomainTemplateDescriptor<T>
        where T : class, IDomainTemplate
    {
        private static Type _TemplateType;

        static GrpcTemplateBuilder()
        {
            var type = typeof(T);
            if (!type.IsInterface)
                throw new NotSupportedException("Not support non interface type as a template.");
            if (type.GetFields().Length != 0)
                throw new NotSupportedException("Interface template could not contains fields.");
            if (type.GetProperties().Length != 0)
                throw new NotSupportedException("Interface template could not contains properties.");
            if (type.GetEvents().Length != 0)
                throw new NotSupportedException("Interface template could not contains events.");
            var typeBuilder = Module.DefineType("Proxy_" + typeof(T).Name, TypeAttributes.Public | TypeAttributes.Class, typeof(GrpcTemplate), new Type[] { type, typeof(IDomainTemplate) });

            //Create constructor ctor(GrpcChannel)
            {
                var constructor = typeBuilder.DefineConstructor(MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, CallingConventions.Standard, new Type[] { typeof(GrpcChannel) });
                var ilGenerator = constructor.GetILGenerator();
                ilGenerator.Emit(OpCodes.Ldarg_0);
                ilGenerator.Emit(OpCodes.Ldarg_1);
                ilGenerator.Emit(OpCodes.Call, _TemplateConstructorInfo);
                ilGenerator.Emit(OpCodes.Ret);
            }

            //Create static constrcutor
            var staticConstructor = typeBuilder.DefineConstructor(MethodAttributes.Static, CallingConventions.Standard, null);
            var staticILGenerator = staticConstructor.GetILGenerator();

            Dictionary<string, int> methodOverrideCount = new Dictionary<string, int>();
            //Implement methods in interface
            foreach (var method in type.GetMethods())
            {
                if (!typeof(Task).IsAssignableFrom(method.ReturnType))
                    throw new NotSupportedException($"Return type of {method.DeclaringType.FullName}.{method.Name}({string.Join(",", method.GetParameters().Select(t => t.ParameterType.Name))}) is not a Task type.");

                //Define method override index
                int methodIndex;
                if (methodOverrideCount.ContainsKey(method.Name))
                    methodIndex = methodOverrideCount[method.Name] = methodOverrideCount[method.Name] + 1;
                else
                    methodIndex = methodOverrideCount[method.Name] = 1;

                var parameters = method.GetParameters();
                //Define request and response type
                Type requestType, responseType, argumentType;
                if (method.ReturnType.IsGenericType)
                    responseType = typeof(DomainGrpcResponse<>).MakeGenericType(method.ReturnType.GetGenericArguments());
                else
                    responseType = typeof(DomainGrpcResponse);
                if (parameters.Length == 0)
                {
                    requestType = typeof(DomainGrpcRequest);
                    argumentType = null;
                }
                else
                {
                    argumentType = DomainGrpcArgumentHelper.GetArgumentType(parameters.Select(t => t.ParameterType).ToArray());
                    requestType = typeof(DomainGrpcRequest<>).MakeGenericType(argumentType);
                }

                //Create static Method<,> field and set value
                var methodField = typeBuilder.DefineField("_Method_" + method.Name + "_" + methodIndex, typeof(Method<,>).MakeGenericType(requestType, responseType), FieldAttributes.Private | FieldAttributes.Static);
                staticILGenerator.Emit(OpCodes.Ldstr, type.Name);
                staticILGenerator.Emit(OpCodes.Ldstr, method.Name + "_" + methodIndex);
                staticILGenerator.Emit(OpCodes.Call, typeof(DomainGrpcMethod<,>).MakeGenericType(requestType, responseType).GetMethod("CreateMethod"));
                staticILGenerator.Emit(OpCodes.Stsfld, methodField);

                //Create method for class
                var methodBuilder = typeBuilder.DefineMethod(method.Name,
                    MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual,
                    method.ReturnType, method.GetParameters().Select(t => t.ParameterType).ToArray());

                //Define parameters
                for (int i = 0; i < parameters.Length; i++)
                {
                    var parameterBuilder = methodBuilder.DefineParameter(i, parameters[i].Attributes, parameters[i].Name);
                    if (parameters[i].HasDefaultValue)
                        parameterBuilder.SetConstant(parameters[i].DefaultValue);
                }
                //Build il codes
                var ilGenerator = methodBuilder.GetILGenerator();

                ilGenerator.Emit(OpCodes.Ldarg_0);
                ilGenerator.Emit(OpCodes.Ldsfld, methodField);
                if (parameters.Length == 0 && method.ReturnType == typeof(Task))
                {
                    ilGenerator.Emit(OpCodes.Call, _InvokeAsyncMethodInfo);
                }
                else if (parameters.Length > 0 && method.ReturnType == typeof(Task))
                {
                    for (int i = 0; i < parameters.Length; i++)
                        ilGenerator.Emit(OpCodes.Ldarg_S, i + 1);
                    ilGenerator.Emit(OpCodes.Newobj, argumentType.GetConstructor(parameters.Select(t => t.ParameterType).ToArray()));
                    ilGenerator.Emit(OpCodes.Call, _InvokeWithArgumentsAsyncMethodInfo.MakeGenericMethod(argumentType));
                }
                else if (parameters.Length == 0 && method.ReturnType != typeof(Task))
                {
                    ilGenerator.Emit(OpCodes.Call, _InvokeWithReturnValueAsyncMethodInfo.MakeGenericMethod(method.ReturnType.GetGenericArguments()));
                }
                else if (parameters.Length > 0 && method.ReturnType != typeof(Task))
                {
                    for (int i = 0; i < parameters.Length; i++)
                        ilGenerator.Emit(OpCodes.Ldarg_S, i + 1);
                    ilGenerator.Emit(OpCodes.Newobj, argumentType.GetConstructor(parameters.Select(t => t.ParameterType).ToArray()));
                    ilGenerator.Emit(OpCodes.Call, _InvokeWithArgumentsAndReturnValueAsyncMethodInfo.MakeGenericMethod(argumentType, method.ReturnType.GetGenericArguments()[0]));
                }

                ilGenerator.Emit(OpCodes.Ret);
            }

            staticILGenerator.Emit(OpCodes.Ret);

            var typeInfo = typeBuilder.CreateTypeInfo();
            _TemplateType = typeInfo.AsType();
        }

        private GrpcChannel _channel;

        public GrpcTemplateBuilder(GrpcChannel channel)
        {
            _channel = channel ?? throw new ArgumentNullException(nameof(channel));
        }

        public T GetTemplate(IDomainContext context)
        {
            var template = (T)Activator.CreateInstance(_TemplateType, _channel);
            template.Context = context;
            return template;
        }
    }
}
