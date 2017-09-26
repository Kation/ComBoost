using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    /// <summary>
    /// 存储提供器。
    /// </summary>
    public interface IStorageProvider
    {
        /// <summary>
        /// 获取存储服务。
        /// </summary>
        /// <returns>返回存储服务。</returns>
        IStorage GetStorage();

        /// <summary>
        /// 根据配置获取存储服务。
        /// </summary>
        /// <param name="name">配置名称。</param>
        /// <returns>返回存储服务。</returns>
        IStorage GetStorage(string name);
    }
}
