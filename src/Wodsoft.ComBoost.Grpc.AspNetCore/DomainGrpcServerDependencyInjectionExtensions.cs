using Grpc.AspNetCore.Server.Model;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using Wodsoft.ComBoost;
using Wodsoft.ComBoost.AspNetCore;
using Wodsoft.ComBoost.Grpc.AspNetCore;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DomainGrpcServerDependencyInjectionExtensions
    {
        public static IComBoostGrpcBuilder AddGrpcServices(this IComBoostAspNetCoreBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton(typeof(IServiceMethodProvider<DomainGrpcDiscoveryService>), typeof(DomainGrpcServiceMethodProvider)));
            return new ComBoostGrpcBuilder(builder.Services);
        }

        public static IComBoostGrpcBuilder AddAuthenticationPassthrough(this IComBoostGrpcBuilder builder)
        {
            builder.Services.TryAddEnumerable(ServiceDescriptor.Scoped<IDomainRpcServerRequestHandler, DomainGrpcServerAuthenticationPassthroughRequestHandler>());
            return builder;
        }
    }
}
