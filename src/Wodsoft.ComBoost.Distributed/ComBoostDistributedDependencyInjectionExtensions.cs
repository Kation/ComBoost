using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Wodsoft.ComBoost;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ComBoostDistributedDependencyInjectionExtensions
    {
        public static IComBoostBuilder AddDistributed(this IComBoostBuilder builder, Action<IComBoostDistributedBuilder> distributedBuilderCongifure)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            if (distributedBuilderCongifure == null)
                throw new ArgumentNullException(nameof(distributedBuilderCongifure));
            var distributedBuilder = new ComBoostDistributedBuilder(builder.Services, builder);
            foreach (var module in builder.Modules)
                if (module is IDomainDistributedModule distributedModule)
                    distributedModule.ConfigureDistributedServices(distributedBuilder);
            distributedBuilderCongifure(distributedBuilder);
            return builder;
        }

        public static IComBoostDistributedBuilder AddRequestHandler<T>(this IComBoostDistributedBuilder builder)
            where T : class, IDomainRpcServerRequestHandler
        {
            builder.Services.TryAddEnumerable(ServiceDescriptor.Scoped<IDomainRpcServerRequestHandler, T>());
            return builder;
        }

        public static IComBoostDistributedBuilder AddResponseHandler<T>(this IComBoostDistributedBuilder builder)
            where T : class, IDomainRpcServerResponseHandler
        {
            builder.Services.TryAddEnumerable(ServiceDescriptor.Scoped<IDomainRpcServerResponseHandler, T>());
            return builder;
        }
    }
}
