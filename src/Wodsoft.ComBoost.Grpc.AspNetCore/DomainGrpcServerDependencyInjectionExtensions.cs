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
        public static IComBoostAspNetCoreBuilder AddGrpcServices(this IComBoostAspNetCoreBuilder builder, Action<DomainGrpcTemplateOptions> optionsConfigure)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton(typeof(IServiceMethodProvider<DomainGrpcDiscoveryService>), typeof(DomainGrpcServiceMethodProvider)));
            if (optionsConfigure == null)
                throw new ArgumentNullException(nameof(optionsConfigure));
            builder.Services.PostConfigure(optionsConfigure);
            return builder;
        }
    }
}
