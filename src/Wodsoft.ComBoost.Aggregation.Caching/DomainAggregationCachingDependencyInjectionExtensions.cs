using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using Wodsoft.ComBoost.Aggregation;
using Wodsoft.ComBoost.Aggregation.Caching;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DomainAggregationCachingDependencyInjectionExtensions
    {
        public static IComBoostAggregationBuilder UseMemoryCache(this IComBoostAggregationBuilder builder, Action<DomainAggregatorMemoryCacheOptions> setupAction = null)
        {
            builder.Services.AddMemoryCache();
            if (setupAction != null)
                builder.Services.PostConfigure(setupAction);
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton(typeof(IDomainAggregatorProvider<,>), typeof(DomainAggregatorMemoryCacheProvider<,>)));
            return new ComBoostAggregationBuilder(builder.Services);
        }

        /// <summary>
        /// Use distributed cache for caching aggregations.<br/>
        /// Must add <see cref="Microsoft.Extensions.Caching.Distributed.IDistributedCache"/> dependency manually.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="setupAction"></param>
        /// <returns></returns>
        public static IComBoostAggregationBuilder UseDistributedCache(this IComBoostAggregationBuilder builder, Action<DomainAggregatorDistributedCacheOptions> setupAction = null)
        {
            if (setupAction != null)
                builder.Services.PostConfigure(setupAction);
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton(typeof(IDomainAggregatorProvider<,>), typeof(DomainAggregatorDistributedCacheProvider<,>)));
            return new ComBoostAggregationBuilder(builder.Services);
        }
    }
}
