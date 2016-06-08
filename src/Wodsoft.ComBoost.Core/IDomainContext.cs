using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    /// <summary>
    /// 领域上下文接口。
    /// </summary>
    public interface IDomainContext : IServiceProvider
    {
        /// <summary>
        /// 获取或设置结果值。
        /// </summary>
        object Result { get; set; }

        /// <summary>
        /// 获取数据字典。
        /// </summary>
        dynamic DataBag { get; }
    }
}
