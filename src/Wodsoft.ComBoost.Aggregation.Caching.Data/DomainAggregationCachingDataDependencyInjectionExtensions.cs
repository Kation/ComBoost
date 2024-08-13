//using Microsoft.Extensions.Caching.Distributed;
//using Microsoft.Extensions.Caching.Memory;
//using Microsoft.Extensions.DependencyInjection.Extensions;
//using Microsoft.Extensions.Options;
//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Threading.Tasks;
//using Wodsoft.ComBoost;
//using Wodsoft.ComBoost.Aggregation;
//using Wodsoft.ComBoost.Aggregation.Caching;
//using Wodsoft.ComBoost.Data;
//using Wodsoft.ComBoost.Data.Distributed;
//using Wodsoft.ComBoost.Data.Entity;

//namespace Microsoft.Extensions.DependencyInjection
//{
//    public static class DomainAggregationCachingDataDependencyInjectionExtensions
//    {
//        public static IComBoostDistributedEventProviderBuilder AddAggregationMemoryCacheObjectChangedHandler<T>(this IComBoostDistributedEventProviderBuilder builder)
//            where T : class
//        {
//            builder.AddDistributedEventHandler<ObjectChangedEventArgs<T>>((context, e) =>
//            {
//                if (e.Keys != null)
//                {
//                    var options = context.DomainContext.GetRequiredService<IOptions<DomainAggregatorMemoryCacheOptions>>();
//                    var cache = context.DomainContext.GetRequiredService<IMemoryCache>();
//                    var keyName = $"{options.Value.Prefix}{typeof(T).FullName}_{string.Join("_", e.Keys)}";
//                    cache.Remove(keyName);
//                }
//                return Task.CompletedTask;
//            });
//            return builder;
//        }

//        public static IComBoostDistributedEventProviderBuilder AddAggregationDistributedCacheObjectChangedHandler<T>(this IComBoostDistributedEventProviderBuilder builder)
//            where T : class
//        {
//            builder.AddDistributedEventHandler<ObjectChangedEventArgs<T>>((context, e) =>
//            {
//                if (e.Keys == null)
//                    return Task.CompletedTask;
//                var options = context.DomainContext.GetRequiredService<IOptions<DomainAggregatorDistributedCacheOptions>>();
//                var cache = context.DomainContext.GetRequiredService<IDistributedCache>();
//                var keyName = $"{options.Value.Prefix}{typeof(T).FullName}_{string.Join("_", e.Keys)}";
//                return cache.RemoveAsync(keyName);
//            });
//            return builder;
//        }
//    }
//}
