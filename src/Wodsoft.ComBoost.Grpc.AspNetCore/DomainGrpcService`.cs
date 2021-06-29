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

        private DomainGrpcTrace _trace;
        private Stopwatch _watch;

        protected void HandleRequest(DomainGrpcRequest request, ServerCallContext context)
        {
            var handlers = Template.Context.GetServices<IDomainRpcServerRequestHandler>();
            foreach (var handler in handlers)
                handler.Handle(request, (DomainGrpcContext)Template.Context);
            if (_options.IsTraceEnabled)
            {
                _trace = new DomainGrpcTrace();
                _trace.StartTime = DateTimeOffset.Now;
                _watch = new Stopwatch();
                _watch.Start();
            }
        }

        protected DomainGrpcResponse<TResult> HandleResponse<TResult>(Task<TResult> task)
        {
            var response = new DomainGrpcResponse<TResult>();
            if (_options.IsTraceEnabled)
            {
                _watch.Stop();
                _trace.EndTime = DateTimeOffset.Now;
                _trace.ElapsedTime = _watch.Elapsed;
                response.Trace = _trace;
            }
            response.OS = Environment.OSVersion.VersionString;
            if (task.Exception == null)
                response.Result = task.Result;
            else
                response.Exception = new DomainGrpcException(task.Exception);
            var handlers = Template.Context.GetServices<IDomainRpcServerResponseHandler>();
            foreach (var handler in handlers)
                handler.Handle(response);
            return response;
        }

        protected DomainGrpcResponse HandleResponse(Task task)
        {
            var response = new DomainGrpcResponse();
            if (_options.IsTraceEnabled)
            {
                _watch.Stop();
                _trace.EndTime = DateTimeOffset.Now;
                _trace.ElapsedTime = _watch.Elapsed;
                response.Trace = _trace;
            }
            response.OS = Environment.OSVersion.VersionString;
            if (task.Exception != null)
                response.Exception = new DomainGrpcException(task.Exception);
            var handlers = Template.Context.GetServices<IDomainRpcServerResponseHandler>();
            foreach (var handler in handlers)
                handler.Handle(response);
            return response;
        }

        internal static readonly Type ServiceType;

        static DomainGrpcService()
        {
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
                Type requestType, argumentType;
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
                var methodBuilder = typeBuilder.DefineMethod(method.Name + "_" + methodIndex, MethodAttributes.Public | MethodAttributes.Virtual);
                methodBuilder.SetParameters(requestType, typeof(ServerCallContext));
                methodBuilder.SetReturnType(typeof(Task<>).MakeGenericType(responseType));

                var methodILGenerator = methodBuilder.GetILGenerator();
                //base.HandleRequest(Request, ServerCallContext);
                methodILGenerator.Emit(OpCodes.Ldarg_0);
                methodILGenerator.Emit(OpCodes.Ldarg_1);
                methodILGenerator.Emit(OpCodes.Ldarg_2);
                methodILGenerator.Emit(OpCodes.Call, typeof(DomainGrpcService<T>).GetMethod(nameof(HandleRequest), BindingFlags.NonPublic | BindingFlags.Instance));


                //_template.{method}(args...)
                methodILGenerator.Emit(OpCodes.Ldarg_0);
                methodILGenerator.Emit(OpCodes.Ldfld, typeof(DomainGrpcService<T>).GetField(nameof(Template), BindingFlags.NonPublic | BindingFlags.Instance));
                //methodILGenerator.Emit(OpCodes.Ldarg_1);
                //methodILGenerator.Emit(OpCodes.Call())
                for (int i = 0; i < parameters.Length; i++)
                {
                    methodILGenerator.Emit(OpCodes.Ldarg_1);
                    methodILGenerator.Emit(OpCodes.Call, requestType.GetProperty("Argument").GetMethod);
                    methodILGenerator.Emit(OpCodes.Call, argumentType.GetProperty("Argument" + (i + 1)).GetMethod);
                }
                methodILGenerator.Emit(OpCodes.Callvirt, method);

                //.ContinueWith(HandleResponse)
                if (method.ReturnType == typeof(Task))
                {
                    methodILGenerator.Emit(OpCodes.Ldarg_0);
                    methodILGenerator.Emit(OpCodes.Ldftn, typeof(DomainGrpcService<T>).GetMethod(nameof(HandleResponse), BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(Task) }, null));
                    methodILGenerator.Emit(OpCodes.Newobj, typeof(Func<Task, DomainGrpcResponse>).GetConstructors()[0]);
                    methodILGenerator.Emit(OpCodes.Call, method.ReturnType.GetMethod("ContinueWith", 1, BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(Func<,>).MakeGenericType(method.ReturnType, Type.MakeGenericMethodParameter(0)) }, null).MakeGenericMethod(typeof(DomainGrpcResponse)));
                }
                else
                {
                    methodILGenerator.Emit(OpCodes.Ldarg_0);
                    var handleMethod = typeof(DomainGrpcService<T>).GetMethod(nameof(HandleResponse), 1, BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(Task<>).MakeGenericType(Type.MakeGenericMethodParameter(0)) }, null).MakeGenericMethod(method.ReturnType.GetGenericArguments());
                    methodILGenerator.Emit(OpCodes.Ldftn, handleMethod);
                    methodILGenerator.Emit(OpCodes.Newobj, typeof(Func<,>).MakeGenericType(method.ReturnType, handleMethod.ReturnType).GetConstructors()[0]);
                    methodILGenerator.Emit(OpCodes.Call, method.ReturnType.GetMethod("ContinueWith", 1, BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(Func<,>).MakeGenericType(method.ReturnType, Type.MakeGenericMethodParameter(0)) }, null).MakeGenericMethod(handleMethod.ReturnType));
                }
                methodILGenerator.Emit(OpCodes.Ret);

                //Create static Method<,> field and set value
                var methodField = typeBuilder.DefineField("_Method_" + method.Name + "_" + methodIndex, typeof(Method<,>).MakeGenericType(requestType, responseType), FieldAttributes.Private | FieldAttributes.Static);
                staticILGenerator.Emit(OpCodes.Ldstr, typeof(T).Name);
                staticILGenerator.Emit(OpCodes.Ldstr, method.Name + "_" + methodIndex);
                staticILGenerator.Emit(OpCodes.Call, typeof(DomainGrpcMethod<,>).MakeGenericType(requestType, responseType).GetMethod("CreateMethod"));
                staticILGenerator.Emit(OpCodes.Stsfld, methodField);
            }

            staticILGenerator.Emit(OpCodes.Ret);

            var typeInfo = typeBuilder.CreateTypeInfo();
            ServiceType = typeInfo.AsType();
        }
    }
}
