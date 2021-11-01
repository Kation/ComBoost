using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Aggregation.Caching
{
    public class DomainAggregatorMemoryCacheProvider<T, TKey> : IDomainAggregatorProvider<T, TKey>
    {
        private IMemoryCache _cache;
        private DomainAggregatorMemoryCacheOptions _options;

        public DomainAggregatorMemoryCacheProvider(IMemoryCache cache, IOptions<DomainAggregatorMemoryCacheOptions> options)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public Task<T> GetAsync(TKey key, DomainAggregatorExecutionPipeline<T> next)
        {
            var keyName = $"{_options.Prefix}{typeof(T).FullName}_{key}";
            var value = _cache.Get<T>(keyName);
            if (value != null || next == null)
                return Task.FromResult(value);
            return next().ContinueWith(task =>
            {
                if (task.Result != null)
                    _cache.Set(keyName, task.Result, new MemoryCacheEntryOptions
                    {
                        SlidingExpiration = _options.ExpireTime
                    });
                return task.Result;
            });
        }
    }
}
