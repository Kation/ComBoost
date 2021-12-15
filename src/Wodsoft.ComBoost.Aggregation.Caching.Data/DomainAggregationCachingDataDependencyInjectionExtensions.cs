using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Wodsoft.ComBoost;
using Wodsoft.ComBoost.Aggregation;
using Wodsoft.ComBoost.Aggregation.Caching;
using Wodsoft.ComBoost.Data;
using Wodsoft.ComBoost.Data.Distributed;
using Wodsoft.ComBoost.Data.Entity;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DomainAggregationCachingDataDependencyInjectionExtensions
    {
        public static IComBoostDistributedBuilder AddAggregationMemoryCacheObjectChangedHandler<T>(this IComBoostDistributedBuilder builder)
            where T : class
        {
            builder.AddDistributedEventHandler<ObjectChangedEventArgs<T>>((context, e) =>
            {
                var options = context.DomainContext.GetService<IOptions<DomainAggregatorMemoryCacheOptions>>();
                var cache = context.DomainContext.GetService<IMemoryCache>();
                var keyName = $"{options.Value.Prefix}{typeof(T).FullName}_{string.Join("_", e.Keys)}";
                cache.Remove(keyName);
                return Task.CompletedTask;
            });
            return builder;
        }

        public static IComBoostDistributedBuilder AddAggregationDistributedCacheObjectChangedHandler<T>(this IComBoostDistributedBuilder builder)
            where T : class
        {
            builder.AddDistributedEventHandler<ObjectChangedEventArgs<T>>((context, e) =>
            {
                var options = context.DomainContext.GetService<IOptions<DomainAggregatorDistributedCacheOptions>>();
                var cache = context.DomainContext.GetService<IDistributedCache>();
                var keyName = $"{options.Value.Prefix}{typeof(T).FullName}_{string.Join("_", e.Keys)}";
                return cache.RemoveAsync(keyName);
            });
            return builder;
        }
    }
}
