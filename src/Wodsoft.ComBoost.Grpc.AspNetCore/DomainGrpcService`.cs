using Grpc.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Grpc.AspNetCore
{
    public class DomainGrpcService<T>
        where T : class, IDomainTemplate
    {
        public DomainGrpcService(T template, IOptions<DomainRpcOptions> options, ILogger<DomainGrpcService<T>> logger)
        {
            Template = template ?? throw new ArgumentNullException(nameof(template));
            _options = options.Value;
            _logger = logger;
        }

        protected readonly T Template;
        private DomainRpcOptions _options;
        private ILogger _logger;

        private DomainGrpcTrace? _trace;
        private Stopwatch? _watch;

        protected async Task HandleRequest(DomainGrpcRequest request, ServerCallContext context)
        {
            var handlers = Template.Context.GetServices<IDomainRpcServerRequestHandler>();
            foreach (var handler in handlers)
                await handler.HandleAsync(request, (DomainGrpcContext)Template.Context);
            if (_options.IsTraceEnabled)
            {
                _trace = new DomainGrpcTrace();
                _trace.StartTime = DateTimeOffset.Now;
                _watch = new Stopwatch();
                _watch.Start();
            }
        }

        protected async Task<DomainGrpcResponse<TResult>> HandleResponse<TResult>(Task<TResult> task)
        {
            var response = new DomainGrpcResponse<TResult>();
            if (_options.IsTraceEnabled)
            {
                _watch!.Stop();
                _trace!.EndTime = DateTimeOffset.Now;
                _trace.ElapsedTime = _watch.Elapsed;
                response.Trace = _trace;
            }
            response.OS = Environment.OSVersion.VersionString;
            if (task.Exception == null)
                response.Result = task.Result;
            else
            {
                _logger.LogError(task.Exception, "Domain service throw a unhandled exception.");
                response.Exception = new DomainGrpcException(task.Exception);
            }
            var handlers = Template.Context.GetServices<IDomainRpcServerResponseHandler>();
            foreach (var handler in handlers)
                await handler.HandleAsync(response);
            return response;
        }

        protected async Task<DomainGrpcResponse> HandleResponse(Task task)
        {
            var response = new DomainGrpcResponse();
            if (_options.IsTraceEnabled)
            {
                _watch!.Stop();
                _trace!.EndTime = DateTimeOffset.Now;
                _trace.ElapsedTime = _watch.Elapsed;
                response.Trace = _trace;
            }
            response.OS = Environment.OSVersion.VersionString;
            if (task.Exception != null)
                response.Exception = new DomainGrpcException(task.Exception);
            var handlers = Template.Context.GetServices<IDomainRpcServerResponseHandler>();
            foreach (var handler in handlers)
                await handler.HandleAsync(response);
            return response;
        }

        internal static Type? ServiceType;
        private static IDomainGrpcMethodBuilder? _MethodBuilder;
        private static MethodInfo _UnwrapMethod = typeof(TaskExtensions).GetMethod("Unwrap", BindingFlags.Public | BindingFlags.Static, null, [typeof(Task<Task>)], null)!;
        private static MethodInfo _UnwrapGenericMethod = typeof(TaskExtensions).GetMethod("Unwrap", 1, BindingFlags.Public | BindingFlags.Static, null, [typeof(Task<>).MakeGenericType(typeof(Task<>).MakeGenericType(Type.MakeGenericMethodParameter(0)))], null)!;
        private static MethodInfo _ExceptionProperty = typeof(Task).GetProperty("Exception")!.GetGetMethod()!;
        private static MethodInfo _FromExceptionMethod = typeof(Task).GetMethod("FromException", 0, BindingFlags.Public | BindingFlags.Static, null, [typeof(Exception)], null)!;
        private static MethodInfo _FromExceptionGenericMethod = typeof(Task).GetMethod("FromException", 1, BindingFlags.Public | BindingFlags.Static, null, [typeof(Exception)], null)!;

        public static IDomainGrpcMethodBuilder MethodBuilder => _MethodBuilder ?? throw new InvalidOperationException("Service not build yet.");

        internal static void Build(IDomainGrpcMethodBuilder grpcMethodBuilder)
        {
            _MethodBuilder = grpcMethodBuilder;

            var templateType = typeof(T);
            TypeBuilder typeBuilder = DomainGrpcService.CreateType(templateType.Name.Trim('I'), typeof(DomainGrpcService<T>));

            //Define constructor
            {
                var constructor = typeBuilder.DefineConstructor(MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, CallingConventions.Standard,
                    new Type[] { templateType, typeof(IOptions<DomainRpcOptions>), typeof(ILogger<DomainGrpcService<T>>) });
                var ilGenerator = constructor.GetILGenerator();
                ilGenerator.Emit(OpCodes.Ldarg_0);
                ilGenerator.Emit(OpCodes.Ldarg_1);
                ilGenerator.Emit(OpCodes.Ldarg_2);
                ilGenerator.Emit(OpCodes.Ldarg_3);
                ilGenerator.Emit(OpCodes.Call, typeof(DomainGrpcService<T>).GetConstructors()[0]);
                ilGenerator.Emit(OpCodes.Ret);
            }

            //Define static constructor
            var staticConstructor = typeBuilder.DefineConstructor(MethodAttributes.Private | MethodAttributes.HideBySig | MethodAttributes.SpecialName |
                MethodAttributes.RTSpecialName | MethodAttributes.Static, CallingConventions.Standard, null);
            var staticILGenerator = staticConstructor.GetILGenerator();

            //Service end point provider
            IDomainGrpcEndPointProvider? serviceEndPointProvider;
            string? serviceName;
            DomainGrpcServiceAttribute? serviceAttr = templateType.GetCustomAttribute<DomainGrpcServiceAttribute>();
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
            //Define methods for template
            foreach (var method in typeof(T).GetTypeInfo().DeclaredMethods.Where(t => t.IsPublic && typeof(Task).IsAssignableFrom(t.ReturnType)))
            {
                //Define method override index
                int methodIndex;
                if (methodOverrideCount.ContainsKey(method.Name))
                    methodIndex = methodOverrideCount[method.Name] = methodOverrideCount[method.Name] + 1;
                else
                    methodIndex = methodOverrideCount[method.Name] = 1;

                //Define request and response type
                Type responseType = method.ReturnType == typeof(Task) ? typeof(DomainGrpcResponse) : typeof(DomainGrpcResponse<>).MakeGenericType(method.ReturnType.GetGenericArguments());
                Type requestType;
                Type? argumentType;
                var parameters = method.GetParameters();
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
                //Define method "Response Method(Request, ServerCallContext)"
                var methodBuilder = typeBuilder.DefineMethod(method.Name + "_" + methodIndex, MethodAttributes.Public);
                methodBuilder.SetParameters(requestType, typeof(ServerCallContext));
                methodBuilder.SetReturnType(typeof(Task<>).MakeGenericType(responseType));

                var methodILGenerator = methodBuilder.GetILGenerator();
                //base.HandleRequest(Request, ServerCallContext);
                methodILGenerator.Emit(OpCodes.Ldarg_0);
                methodILGenerator.Emit(OpCodes.Ldarg_1);
                methodILGenerator.Emit(OpCodes.Ldarg_2);
                methodILGenerator.Emit(OpCodes.Call, typeof(DomainGrpcService<T>).GetMethod(nameof(HandleRequest), BindingFlags.NonPublic | BindingFlags.Instance)!);

                var invokeMethodBuilder = typeBuilder.DefineMethod(method.Name + "_" + methodIndex + "_Invoker", MethodAttributes.Private);
                if (parameters.Length == 0)
                    invokeMethodBuilder.SetParameters(typeof(Task));
                else
                    invokeMethodBuilder.SetParameters(typeof(Task), typeof(object));
                invokeMethodBuilder.SetReturnType(method.ReturnType);
                var invokeILGenerator = invokeMethodBuilder.GetILGenerator();
                var invokeLabel = invokeILGenerator.DefineLabel();
                //if (task.Exception != null) return Task.FromException(task.Exception);
                //_template.{method}(args...)
                invokeILGenerator.Emit(OpCodes.Ldarg_1);
                invokeILGenerator.Emit(OpCodes.Call, _ExceptionProperty);
                invokeILGenerator.Emit(OpCodes.Brfalse_S, invokeLabel);
                invokeILGenerator.Emit(OpCodes.Ldarg_1);
                invokeILGenerator.Emit(OpCodes.Call, _ExceptionProperty);
                if (method.ReturnType == typeof(Task))
                    invokeILGenerator.Emit(OpCodes.Call, _FromExceptionMethod);
                else
                    invokeILGenerator.Emit(OpCodes.Call, _FromExceptionGenericMethod.MakeGenericMethod(method.ReturnType.GetGenericArguments()[0]));
                invokeILGenerator.Emit(OpCodes.Ret);
                invokeILGenerator.MarkLabel(invokeLabel);
                invokeILGenerator.Emit(OpCodes.Ldarg_0);
                invokeILGenerator.Emit(OpCodes.Ldfld, typeof(DomainGrpcService<T>).GetField(nameof(Template), BindingFlags.NonPublic | BindingFlags.Instance)!);
                for (int i = 0; i < parameters.Length; i++)
                {
                    invokeILGenerator.Emit(OpCodes.Ldarg_2);
                    invokeILGenerator.Emit(OpCodes.Call, argumentType!.GetProperty("Argument" + (i + 1))!.GetMethod!);
                }
                invokeILGenerator.Emit(OpCodes.Callvirt, method);
                invokeILGenerator.Emit(OpCodes.Ret);

                //.ContinueWith(Invoker, Arguments).Unwrap()
                methodILGenerator.Emit(OpCodes.Ldarg_0);
                methodILGenerator.Emit(OpCodes.Ldftn, invokeMethodBuilder);
                if (parameters.Length == 0)
                {
                    methodILGenerator.Emit(OpCodes.Newobj, typeof(Func<,>).MakeGenericType([typeof(Task), method.ReturnType]).GetConstructors()[0]);
                    methodILGenerator.Emit(OpCodes.Call, typeof(Task).GetMethod("ContinueWith", 1, BindingFlags.Public | BindingFlags.Instance, null,
                        new Type[] { typeof(Func<,>).MakeGenericType(typeof(Task), Type.MakeGenericMethodParameter(0)) }, null)!.MakeGenericMethod(method.ReturnType));
                }
                else
                {
                    methodILGenerator.Emit(OpCodes.Newobj, typeof(Func<,,>).MakeGenericType([typeof(Task), typeof(object), method.ReturnType]).GetConstructors()[0]);
                    methodILGenerator.Emit(OpCodes.Ldarg_1);
                    methodILGenerator.Emit(OpCodes.Call, requestType.GetProperty("Argument")!.GetMethod!);
                    methodILGenerator.Emit(OpCodes.Call, typeof(Task).GetMethod("ContinueWith", 1, BindingFlags.Public | BindingFlags.Instance, null,
                        new Type[] { typeof(Func<,,>).MakeGenericType(typeof(Task), typeof(object), Type.MakeGenericMethodParameter(0)), typeof(object) }, null)!.MakeGenericMethod(method.ReturnType));
                }

                //.ContinueWith(HandleResponse).Unwrap()
                if (method.ReturnType == typeof(Task))
                {
                    methodILGenerator.Emit(OpCodes.Call, _UnwrapMethod);
                    methodILGenerator.Emit(OpCodes.Ldarg_0);
                    methodILGenerator.Emit(OpCodes.Ldftn, typeof(DomainGrpcService<T>).GetMethod(nameof(HandleResponse), BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(Task) }, null)!);
                    methodILGenerator.Emit(OpCodes.Newobj, typeof(Func<Task, DomainGrpcResponse>).GetConstructors()[0]);
                    methodILGenerator.Emit(OpCodes.Call, method.ReturnType.GetMethod("ContinueWith", 1, BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(Func<,>).MakeGenericType(method.ReturnType, Type.MakeGenericMethodParameter(0)) }, null)!.MakeGenericMethod(typeof(Task<DomainGrpcResponse>)));
                }
                else
                {
                    methodILGenerator.Emit(OpCodes.Call, _UnwrapGenericMethod.MakeGenericMethod(method.ReturnType.GetGenericArguments()[0]));
                    methodILGenerator.Emit(OpCodes.Ldarg_0);
                    var handleMethod = typeof(DomainGrpcService<T>).GetMethod(nameof(HandleResponse), 1, BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(Task<>).MakeGenericType(Type.MakeGenericMethodParameter(0)) }, null)!.MakeGenericMethod(method.ReturnType.GetGenericArguments());
                    methodILGenerator.Emit(OpCodes.Ldftn, handleMethod);
                    methodILGenerator.Emit(OpCodes.Newobj, typeof(Func<,>).MakeGenericType(method.ReturnType, handleMethod.ReturnType).GetConstructors()[0]);
                    methodILGenerator.Emit(OpCodes.Call, method.ReturnType.GetMethod("ContinueWith", 1, BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(Func<,>).MakeGenericType(method.ReturnType, Type.MakeGenericMethodParameter(0)) }, null)!.MakeGenericMethod(handleMethod.ReturnType));
                }
                methodILGenerator.Emit(OpCodes.Call, _UnwrapGenericMethod.MakeGenericMethod(responseType));
                methodILGenerator.Emit(OpCodes.Ret);

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
                        var methodEndPointProvider = (IDomainGrpcEndPointProvider)Activator.CreateInstance(methodAttr.EndPointProvider)!;
                        methodEndPointProvider.GetEndPoint(method, out methodServiceName, out methodName);
                    }
                }
                //Create static Method<,> field and set value
                var methodField = typeBuilder.DefineField("_Method_" + method.Name + "_" + methodIndex, typeof(Method<,>).MakeGenericType(requestType, responseType), FieldAttributes.Private | FieldAttributes.Static);
                staticILGenerator.Emit(OpCodes.Call, typeof(DomainGrpcService<T>).GetProperty("MethodBuilder", BindingFlags.Public | BindingFlags.Static)!.GetMethod!);
                staticILGenerator.Emit(OpCodes.Ldstr, methodServiceName ?? serviceName ?? DomainService.GetServiceName(typeof(T)));
                staticILGenerator.Emit(OpCodes.Ldstr, methodName ?? (method.Name + "_" + methodIndex));
                staticILGenerator.Emit(OpCodes.Callvirt, typeof(IDomainGrpcMethodBuilder).GetMethod("CreateMethod")!.MakeGenericMethod(requestType, responseType));
                staticILGenerator.Emit(OpCodes.Stsfld, methodField);
            }

            staticILGenerator.Emit(OpCodes.Ret);

            var typeInfo = typeBuilder.CreateTypeInfo()!;
            ServiceType = typeInfo.AsType();
        }
    }
}
