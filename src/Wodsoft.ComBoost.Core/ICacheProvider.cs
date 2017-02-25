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
    }
}
