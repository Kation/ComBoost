using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
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
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IHostedService, DomainDistributedEventService>());
            distributedBuilderCongifure(new ComBoostDistributedBuilder(builder.Services));
            return builder;
        }
    }
}
