using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    public static class CacheExtensions
    {
        public static async Task<T> GetAsync<T>(this ICache cache, string name)
        {
            return (T)await cache.GetAsync(name, typeof(T));
        }
        
        public static async Task<T> GetOrSetAsync<T>(this ICache cache, string name, Func<string, T> getEntryDelegate, TimeSpan? expireTime)
        {
            if (cache == null)
                throw new ArgumentNullException(nameof(cache));
            if (getEntryDelegate == null)
                throw new ArgumentNullException(nameof(getEntryDelegate));
            var entry = await cache.GetAsync<T>(name);
            if (entry == null)
            {
                entry = getEntryDelegate(name);
                await cache.SetAsync(name, entry, expireTime);
            }
            return entry;
        }

        //private static async Task<ICacheEntry> SetCache(ICache cache, ICacheEntry entry)
        //{
        //    if (entry == null)
        //        throw new ArgumentNullException(nameof(entry), "返回的缓存不能为空。");
        //    if (entry.Value == null)
        //        throw new InvalidOperationException("缓存值不能为空。");
        //    if (entry.ExpiredDate < DateTime.Now)
        //        throw new InvalidOperationException("该缓存已过期。");
        //    await cache.SetAsync(entry);
        //    return entry;
        //}
    }
}
