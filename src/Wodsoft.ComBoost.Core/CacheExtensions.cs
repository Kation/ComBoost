using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    /// <summary>
    /// 缓存扩展。
    /// </summary>
    public static class CacheExtensions
    {
        /// <summary>
        /// 异步获取。
        /// </summary>
        /// <typeparam name="T">值类型。</typeparam>
        /// <param name="cache">缓存。</param>
        /// <param name="name">键名称。</param>
        /// <returns></returns>
        public static async Task<T> GetAsync<T>(this ICache cache, string name)
        {
            return (T)await cache.GetAsync(name, typeof(T));
        }

        /// <summary>
        /// 异步获取或设置。
        /// </summary>
        /// <typeparam name="T">值类型。</typeparam>
        /// <param name="cache">缓存。</param>
        /// <param name="name">键名称。</param>
        /// <param name="getEntryDelegate">获取值的委托。</param>
        /// <param name="expireTime">过期时间。</param>
        /// <returns></returns>
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
    }
}
