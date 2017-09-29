using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    /// <summary>
    /// 领域服务访问器。
    /// </summary>
    public interface IDomainServiceAccessor
    {
        /// <summary>
        /// 获取或设置当前领域服务。
        /// </summary>
        IDomainService DomainService { get; set; }
    }
}
