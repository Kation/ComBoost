using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    /// <summary>
    /// 领域上下文接口。
    /// </summary>
    public interface IDomainContext : IServiceProvider
    {
        /// <summary>
        /// 服务取消令牌。
        /// </summary>
        CancellationToken ServiceAborted { get; }

        /// <summary>
        /// 获取数据字典。
        /// </summary>
        dynamic DataBag { get; }

        /// <summary>
        /// 获取领域服务过滤器列表。
        /// </summary>
        IList<IDomainServiceFilter> Filter { get; }

        /// <summary>
        /// 获取领域服务选项。
        /// </summary>
        IDomainServiceOptions Options { get; }

        /// <summary>
        /// 领域事件管理器。
        /// </summary>
        DomainServiceEventManager EventManager { get; }     
    }
}
