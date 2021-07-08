using Grpc.Net.Client;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
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
            ComBoostGrpcBuilder grpcBuilder = new ComBoostGrpcBuilder(builder.Services);
            builderConfigure(grpcBuilder);
            return builder;
        }

        public static IComBoostGrpcBuilder AddAuthenticationPassthrough(this IComBoostGrpcBuilder builder)
        {
            builder.Services.TryAddEnumerable(ServiceDescriptor.Transient<IDomainRpcClientRequestHandler, DomainGrpcClientAuthenticationPassthroughRequestHandler>());
            return builder;
        }

        public static IComBoostGrpcServiceBuilder AddService(this IComBoostGrpcBuilder builder, Uri address)
        {
            return builder.AddService(address, sp => new GrpcChannelOptions());
        }

        public static IComBoostGrpcServiceBuilder AddService(this IComBoostGrpcBuilder builder, Uri address, GrpcChannelOptions options)
        {
            return builder.AddService(address, sp => options);
        }
    }
}
