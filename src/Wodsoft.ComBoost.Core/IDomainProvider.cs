using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    /// <summary>
    /// 领域提供器接口。
    /// </summary>
    public interface IDomainProvider
    {
        /// <summary>
        /// 获取领域服务。
        /// </summary>
        /// <param name="name">服务名。</param>
        /// <returns>返回领域服务。</returns>
        IDomainService GetService(string name);
    }
}
