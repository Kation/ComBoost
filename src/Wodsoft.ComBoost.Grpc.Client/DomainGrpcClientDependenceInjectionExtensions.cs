using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Wodsoft.ComBoost;
using Wodsoft.ComBoost.Grpc.Client;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DomainGrpcClientDependenceInjectionExtensions
    {
        public static IComBoostBuilder AddGrpcService(this IComBoostBuilder builder, Action<IComBoostGrpcBuilder> builderConfigure)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            if (builderConfigure == null)
                throw new ArgumentNullException(nameof(builderConfigure));
            ComBoostGrpcBuilder grpcBuilder = new ComBoostGrpcBuilder(builder.Services, builder);
            builderConfigure(grpcBuilder);
            return builder;
        }

        public static IComBoostGrpcBuilder AddAuthenticationPassthrough(this IComBoostGrpcBuilder builder)
        {
            builder.Services.TryAddEnumerable(ServiceDescriptor.Transient<IDomainRpcClientRequestHandler, DomainGrpcClientAuthenticationPassthroughRequestHandler>());
            return builder;
        }

        public static IComBoostGrpcBuilder UseCallOptionsHandler<T>(this IComBoostGrpcBuilder builder)
            where T : IDomainGrpcCallOptionsHandler, new()
        {
            return builder.UseCallOptionsHandler(new T());
        }

        public static IComBoostGrpcServiceBuilder AddService(this IComBoostGrpcBuilder builder, Uri address)
        {
            return builder.AddService(address, sp => new GrpcChannelOptions());
        }

        public static IComBoostGrpcServiceBuilder AddService(this IComBoostGrpcBuilder builder, Uri address, GrpcChannelOptions options)
        {
            return builder.AddService(address, sp => options);
        }

        public static IComBoostGrpcServiceBuilder UseCallOptionsHandler<T>(this IComBoostGrpcServiceBuilder builder)
            where T : IDomainGrpcCallOptionsHandler, new()
        {
            return builder.UseCallOptionsHandler(new T());
        }

        private readonly static MethodInfo _UseTemplateMethod = typeof(IComBoostGrpcServiceBuilder).GetMethod("UseTemplate");
        public static IComBoostGrpcServiceBuilder UseTemplateInAssembly(this IComBoostGrpcServiceBuilder builder, string serviceName, Assembly assembly, CallOptions callOptions = default)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            if (serviceName == null)
                throw new ArgumentNullException(nameof(serviceName));
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));
            foreach (var type in assembly.GetTypes())
            {
                if (type.IsInterface && !type.IsGenericTypeDefinition && type.GetInterfaces().Any(t => t == typeof(IDomainTemplate)))
                {
                    var attr = type.GetCustomAttribute<DomainDistributedServiceAttribute>();
                    if (attr != null && attr.ServiceName == serviceName)
                        _UseTemplateMethod.MakeGenericMethod(type).Invoke(builder, new object[] { callOptions });
                }
            }
            return builder;
        }

        public static IComBoostGrpcServiceBuilder UseTemplateInAssembly(this IComBoostGrpcServiceBuilder builder, Assembly assembly, CallOptions callOptions = default)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));
            foreach (var type in assembly.GetTypes())
            {
                if (type.IsInterface && !type.IsGenericTypeDefinition && type.GetInterfaces().Any(t => t == typeof(IDomainTemplate)))
                {
                    _UseTemplateMethod.MakeGenericMethod(type).Invoke(builder, new object[] { callOptions });
                }
            }
            return builder;
        }
    }
}
