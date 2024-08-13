using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Aggregation.Caching
{
    public class DomainAggregatorDistributedCacheProvider<T> : IDomainAggregatorProvider<T>
    {
        private IDistributedCache _cache;
        private DomainAggregatorDistributedCacheOptions _options;

        public DomainAggregatorDistributedCacheProvider(IDistributedCache cache, IOptions<DomainAggregatorDistributedCacheOptions> options)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task<T?> GetAsync(object[] keys, DomainAggregatorExecutionPipeline<T>? next)
        {
            var keyName = $"{_options.Prefix}{typeof(T).FullName}_{string.Join("_", keys)}";
            var data = await _cache.GetAsync(keyName);
            if (data != null)
            {
                await _cache.RefreshAsync(keyName);
                return JsonSerializer.Deserialize<T>(data);
            }
            if (next == null)
                return default;
            var value = await next();
            if (value != null)
                await _cache.SetAsync(keyName, JsonSerializer.SerializeToUtf8Bytes(value), new DistributedCacheEntryOptions
                {
                    SlidingExpiration = _options.ExpireTime
                });
            return value;
        }
    }
}
