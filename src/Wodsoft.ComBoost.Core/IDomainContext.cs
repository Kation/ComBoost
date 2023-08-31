using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
        IList<IDomainServiceFilter> Filters { get; }

        /// <summary>
        /// 获取领域事件管理器。
        /// </summary>
        DomainServiceEventManager EventManager { get; }

        /// <summary>
        /// 获取值提供器。
        /// </summary>
        IValueProvider ValueProvider { get; }

        /// <summary>
        /// 获取用户主体声明。
        /// </summary>
        ClaimsPrincipal User { get; }
    }
}
