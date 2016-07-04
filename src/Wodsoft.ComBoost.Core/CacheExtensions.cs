using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    public static class CacheExtensions
    {
        public static async Task<CacheEntry> GetOrSetAsync(this ICache cache, string name, Func<string, CacheEntry> getEntryDelegate)
        {
            if (cache == null)
                throw new ArgumentNullException(nameof(cache));
            if (getEntryDelegate == null)
                throw new ArgumentNullException(nameof(getEntryDelegate));
            var entry = await cache.GetAsync(name);
            if (entry == null)
                return await SetCache(cache, getEntryDelegate(name));
            if (entry.ExpiredDate < DateTime.Now)
                return await SetCache(cache, getEntryDelegate(name));
            return entry;
        }

        private static async Task<CacheEntry> SetCache(ICache cache, CacheEntry entry)
        {
            if (entry == null)
                throw new ArgumentNullException(nameof(entry), "返回的缓存不能为空。");
            if (entry.Value == null)
                throw new InvalidOperationException("缓存值不能为空。");
            if (entry.ExpiredDate < DateTime.Now)
                throw new InvalidOperationException("该缓存已过期。");
            await cache.SetAsync(entry);
            return entry;
        }
    }
}
