using Grpc.AspNetCore.Server.Model;
using Grpc.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Grpc.AspNetCore
{
    public class DomainGrpcServiceMethodProvider : IServiceMethodProvider<DomainGrpcDiscoveryService>
    {
        private DomainGrpcTemplateOptions _options;

        public DomainGrpcServiceMethodProvider(IOptions<DomainGrpcTemplateOptions> options)
        {
            _options = options.Value;
        }

        private static MethodInfo _CreateInstanceMethodInfo = typeof(ActivatorUtilities).GetMethod("CreateInstance", 1, BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(IServiceProvider), typeof(object[]) }, null);
        private static PropertyInfo _ServicesPropertyInfo = typeof(DomainGrpcDiscoveryService).GetProperty("Services");
        private static ConstructorInfo _GrpcContextConstructorInfo = typeof(DomainGrpcContext).GetConstructors()[0];
        private static MethodInfo _GetServiceMethodInfo = typeof(ServiceProviderServiceExtensions).GetMethod("GetRequiredService", 1, BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(IServiceProvider) }, null);


        public void OnServiceMethodDiscovery(ServiceMethodProviderContext<DomainGrpcDiscoveryService> context)
        {
            var contextType = context.GetType();
            foreach (var type in _options.Types)
            {
                var serviceType = (Type)typeof(DomainGrpcService<>).MakeGenericType(type).GetField("ServiceType", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
                foreach (var method in serviceType.GetTypeInfo().DeclaredMethods)
                {
                    if (!method.IsPublic || method.IsStatic)
                        continue;
                    var methodField = serviceType.GetField("_Method_" + method.Name, BindingFlags.NonPublic | BindingFlags.Static);
                    var methodValue = methodField.GetValue(null);
                    var addMethod = contextType.GetMethod("AddUnaryMethod").MakeGenericMethod(methodField.FieldType.GetGenericArguments());

                    var discoveryServiceParameter = Expression.Parameter(typeof(DomainGrpcDiscoveryService));
                    var requestParameter = Expression.Parameter(methodField.FieldType.GetGenericArguments()[0]);
                    var contextParameter = Expression.Parameter(typeof(ServerCallContext));
                    var responseTarget = Expression.Label(typeof(Task<>).MakeGenericType(methodField.FieldType.GetGenericArguments()[1]));
                    var serviceVariable = Expression.Variable(serviceType);
                    var domainContextVaiable = Expression.Variable(typeof(IDomainContext));
                    var templateDescriptorVariable = Expression.Variable(typeof(IDomainTemplateDescriptor<>).MakeGenericType(type));
                    var templateVariable = Expression.Variable(type);
                    var body = Expression.Block(
                        new ParameterExpression[] { serviceVariable, domainContextVaiable, templateDescriptorVariable, templateVariable },
                        //var domainContext = new DomainGrpcContext(request, context);
                        Expression.Assign(domainContextVaiable, Expression.New(_GrpcContextConstructorInfo, requestParameter, contextParameter)),
                        //var templateDescriptor = domainGrpcDiscoveryService.Services.GetService<IDomainTemplateDescriptor<T>>();
                        Expression.Assign(templateDescriptorVariable, Expression.Call(_GetServiceMethodInfo.MakeGenericMethod(templateDescriptorVariable.Type), domainContextVaiable)),
                        //var template = templateDescriptor.GetTemplate(domainContext);
                        Expression.Assign(templateVariable, Expression.Call(templateDescriptorVariable, templateDescriptorVariable.Type.GetMethod("GetTemplate"), domainContextVaiable)),
                        //var service = domainGrpcDiscoveryService.Services.GetService<DomainGrpcService<T>>();
                        Expression.Assign(serviceVariable, Expression.Call(_CreateInstanceMethodInfo.MakeGenericMethod(serviceType), Expression.Property(discoveryServiceParameter, _ServicesPropertyInfo), Expression.NewArrayInit(typeof(object), templateVariable))),
                        //service.{Method}(request, context);
                        Expression.Label(responseTarget, Expression.Call(serviceVariable, method, requestParameter, contextParameter))
                    );
                    var invoker = Expression.Lambda(addMethod.GetParameters()[2].ParameterType, body, discoveryServiceParameter, requestParameter, contextParameter).Compile();
                    addMethod.Invoke(context, new object[] { methodValue, Array.Empty<object>(), invoker });
                }
            }
        }
    }
}
