using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    /// <summary>
    /// 缓存。
    /// </summary>
    public interface ICache
    {
        /// <summary>
        /// 异步获取。
        /// </summary>
        /// <param name="name">键名称。</param>
        /// <param name="valueType">值类型。</param>
        /// <returns>返回异步结果。</returns>
        Task<object> GetAsync(string name, Type valueType);

        /// <summary>
        /// 异步删除。
        /// </summary>
        /// <param name="name">键名称。</param>
        /// <returns>返回异步结果。</returns>
        Task<bool> DeleteAsync(string name);

        /// <summary>
        /// 异步设置。
        /// </summary>
        /// <param name="name">键名称。</param>
        /// <param name="value">值。</param>
        /// <param name="expireTime">过期时间。</param>
        /// <returns>返回异步任务。</returns>
        Task SetAsync(string name, object value, TimeSpan? expireTime);
    }
}
