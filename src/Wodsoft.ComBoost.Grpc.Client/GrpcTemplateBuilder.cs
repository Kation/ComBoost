using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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

        protected internal static ConstructorInfo _TemplateConstructorInfo = typeof(GrpcTemplate).GetConstructor(new Type[] { typeof(GrpcChannel), typeof(CallOptions) });
        protected internal static MethodInfo _InvokeAsyncMethodInfo = typeof(GrpcTemplate).GetMethod("InvokeAsync", BindingFlags.NonPublic | BindingFlags.Instance);
        protected internal static MethodInfo _InvokeWithReturnValueAsyncMethodInfo = typeof(GrpcTemplate).GetMethod("InvokeWithReturnValueAsync", BindingFlags.NonPublic | BindingFlags.Instance);
        protected internal static MethodInfo _InvokeWithArgumentsAsyncMethodInfo = typeof(GrpcTemplate).GetMethod("InvokeWithArgumentsAsync", BindingFlags.NonPublic | BindingFlags.Instance);
        protected internal static MethodInfo _InvokeWithArgumentsAndReturnValueAsyncMethodInfo = typeof(GrpcTemplate).GetMethod("InvokeWithArgumentsAndReturnValueAsync", BindingFlags.NonPublic | BindingFlags.Instance);

        public class GrpcTemplate : IDomainTemplate
        {
            private CallOptions _callOptions;

            public GrpcTemplate(GrpcChannel channel, CallOptions callOptions)
            {
                Channel = channel ?? throw new ArgumentNullException(nameof(channel));
                _callOptions = callOptions;
            }

            [AllowNull]
            public IDomainContext Context { get; set; }

            public GrpcChannel Channel { get; }

            public DomainLifetimeStrategy? OverrideLifetimeStrategy { get => null; set => throw new NotSupportedException("Grpc client doesn't support override lifetime strategy."); }

            protected async Task InvokeAsync(Method<DomainGrpcRequest, DomainGrpcResponse> method)
            {
                DomainGrpcRequest request = new DomainGrpcRequest();
                await HandleRequestAsync(request);
                var invoker = Channel.CreateCallInvoker();
                var response = await invoker.AsyncUnaryCall(method, null, _callOptions, request);
                await HandleResponseAsync(response);
            }

            protected async Task<TValue?> InvokeWithReturnValueAsync<TValue>(Method<DomainGrpcRequest, DomainGrpcResponse<TValue>> method)
            {
                DomainGrpcRequest request = new DomainGrpcRequest();
                await HandleRequestAsync(request);
                var invoker = Channel.CreateCallInvoker();
                var response = await invoker.AsyncUnaryCall(method, null, _callOptions, request);
                await HandleResponseAsync(response);
                return response.Result;
            }

            protected async Task InvokeWithArgumentsAsync<TArgs>(Method<DomainGrpcRequest<TArgs>, DomainGrpcResponse> method, TArgs args)
                where TArgs : new()
            {
                DomainGrpcRequest<TArgs> request = new DomainGrpcRequest<TArgs>();
                request.Argument = args;
                await HandleRequestAsync(request);
                var invoker = Channel.CreateCallInvoker();
                var response = await invoker.AsyncUnaryCall(method, null, _callOptions, request);
                await HandleResponseAsync(response);
            }

            protected async Task<TValue?> InvokeWithArgumentsAndReturnValueAsync<TArgs, TValue>(Method<DomainGrpcRequest<TArgs>, DomainGrpcResponse<TValue>> method, TArgs args)
                where TArgs : new()
            {
                DomainGrpcRequest<TArgs> request = new DomainGrpcRequest<TArgs>();
                request.Argument = args;
                await HandleRequestAsync(request);
                var invoker = Channel.CreateCallInvoker();
                var response = await invoker.AsyncUnaryCall(method, null, _callOptions, request);
                await HandleResponseAsync(response);
                return response.Result;
            }

            private async Task HandleRequestAsync(DomainGrpcRequest request)
            {
                request.OS = Environment.OSVersion.ToString();
                var handlers = Context.GetServices<IDomainRpcClientRequestHandler>();
                foreach (var handler in handlers)
                    await handler.HandleAsync(request, Context);
            }

            private async Task HandleResponseAsync(DomainGrpcResponse response)
            {
                var handlers = Context.GetServices<IDomainRpcClientResponseHandler>();
                foreach (var handler in handlers)
                    await handler.HandleAsync(response);
                if (response.Exception != null)
                    throw new DomainGrpcInvokeException(response.Exception);
            }
        }
    }

    public class GrpcTemplateBuilder<T> : GrpcTemplateBuilder, IDomainTemplateDescriptor<T>
        where T : class, IDomainTemplate
    {
        private static Type? _TemplateType;
        private static IDomainGrpcMethodBuilder? _MethodBuilder;

        public static IDomainGrpcMethodBuilder MethodBuilder => _MethodBuilder ?? throw new InvalidOperationException("Template not build yet.");

        internal static void Build(IDomainGrpcMethodBuilder grpcMethodBuilder)
        {
            _MethodBuilder = grpcMethodBuilder;
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
                var constructor = typeBuilder.DefineConstructor(MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, CallingConventions.Standard, new Type[] { typeof(GrpcChannel), typeof(CallOptions) });
                var ilGenerator = constructor.GetILGenerator();
                ilGenerator.Emit(OpCodes.Ldarg_0);
                ilGenerator.Emit(OpCodes.Ldarg_1);
                ilGenerator.Emit(OpCodes.Ldarg_2);
                ilGenerator.Emit(OpCodes.Call, _TemplateConstructorInfo);
                ilGenerator.Emit(OpCodes.Ret);
            }

            //Create static constrcutor
            var staticConstructor = typeBuilder.DefineConstructor(MethodAttributes.Static, CallingConventions.Standard, null);
            var staticILGenerator = staticConstructor.GetILGenerator();

            //Service end point provider
            IDomainGrpcEndPointProvider? serviceEndPointProvider;
            string? serviceName;
            DomainGrpcServiceAttribute? serviceAttr = type.GetCustomAttribute<DomainGrpcServiceAttribute>();
            if (serviceAttr == null)
            {
                serviceName = null;
                serviceEndPointProvider = null;
            }
            else
            {
                serviceName = serviceAttr.ServiceName;
                if (serviceAttr.EndPointProvider == null)
                    serviceEndPointProvider = null;
                else
                    serviceEndPointProvider = (IDomainGrpcEndPointProvider)Activator.CreateInstance(serviceAttr.EndPointProvider)!;
            }

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
                Type requestType, responseType;
                Type? argumentType;
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

                //Method end point provider
                string? methodServiceName, methodName;
                DomainGrpcMethodAttribute? methodAttr = method.GetCustomAttribute<DomainGrpcMethodAttribute>();
                if (methodAttr == null)
                {
                    methodServiceName = null;
                    methodName = null;
                    if (serviceEndPointProvider != null)
                        serviceEndPointProvider.GetEndPoint(method, out methodServiceName, out methodName);
                }
                else
                {
                    if (methodAttr.EndPointProvider == null)
                    {
                        methodServiceName = methodAttr.ServiceName;
                        methodName = methodAttr.MethodName;
                    }
                    else
                    {
                        var methodEndPointProvider = (IDomainGrpcEndPointProvider)Activator.CreateInstance(methodAttr.EndPointProvider);
                        methodEndPointProvider.GetEndPoint(method, out methodServiceName, out methodName);
                    }
                }
                //Create static Method<,> field and set value
                var methodField = typeBuilder.DefineField("_Method_" + method.Name + "_" + methodIndex, typeof(Method<,>).MakeGenericType(requestType, responseType), FieldAttributes.Private | FieldAttributes.Static);
                staticILGenerator.Emit(OpCodes.Call, typeof(GrpcTemplateBuilder<T>).GetProperty("MethodBuilder", BindingFlags.Public | BindingFlags.Static).GetMethod);
                staticILGenerator.Emit(OpCodes.Ldstr, methodServiceName ?? serviceName ?? DomainService.GetServiceName(type));
                staticILGenerator.Emit(OpCodes.Ldstr, methodName ?? (method.Name + "_" + methodIndex));
                staticILGenerator.Emit(OpCodes.Callvirt, typeof(IDomainGrpcMethodBuilder).GetMethod("CreateMethod").MakeGenericMethod(requestType, responseType));
                staticILGenerator.Emit(OpCodes.Stsfld, methodField);

                //Create method for class
                var methodBuilder = typeBuilder.DefineMethod(method.Name,
                    MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual,
                    method.ReturnType, method.GetParameters().Select(t => t.ParameterType).ToArray());

                //Define parameters
                for (int i = 0; i < parameters.Length; i++)
                {
                    var parameterBuilder = methodBuilder.DefineParameter(i + 1, parameters[i].Attributes, parameters[i].Name);
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
                    ilGenerator.Emit(OpCodes.Newobj, argumentType!.GetConstructor(parameters.Select(t => t.ParameterType).ToArray()));
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
                    ilGenerator.Emit(OpCodes.Newobj, argumentType!.GetConstructor(parameters.Select(t => t.ParameterType).ToArray()));
                    ilGenerator.Emit(OpCodes.Call, _InvokeWithArgumentsAndReturnValueAsyncMethodInfo.MakeGenericMethod(argumentType, method.ReturnType.GetGenericArguments()[0]));
                }

                ilGenerator.Emit(OpCodes.Ret);
            }

            staticILGenerator.Emit(OpCodes.Ret);

            var typeInfo = typeBuilder.CreateTypeInfo();
            _TemplateType = typeInfo.AsType();
        }

        private GrpcChannel _channel;
        private CallOptions _callOptions;

        public GrpcTemplateBuilder(GrpcChannel channel, CallOptions callOptions)
        {
            _channel = channel ?? throw new ArgumentNullException(nameof(channel));
            _callOptions = callOptions;
        }

        public T GetTemplate(IDomainContext context)
        {
            var template = (T)Activator.CreateInstance(_TemplateType!, _channel, _callOptions);
            template.Context = context;
            return template;
        }
    }
}
