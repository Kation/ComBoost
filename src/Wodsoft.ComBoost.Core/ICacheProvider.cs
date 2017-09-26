using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    /// <summary>
    /// 缓存提供器。
    /// </summary>
    public interface ICacheProvider
    {
        /// <summary>
        /// 获取缓存。
        /// </summary>
        /// <returns></returns>
        ICache GetCache();

        /// <summary>
        /// 根据名称获取缓存。
        /// </summary>
        /// <param name="name">缓存名称。</param>
        /// <returns></returns>
        ICache GetCache(string name);
    }
}
