using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    /// <summary>
    /// 领域服务区事件管理器接口。
    /// </summary>
    public interface IDomainServiceEventManager
    {
        #region 关联事件

        /// <summary>
        /// 添加事件处理器。
        /// </summary>
        /// <typeparam name="T">事件参数类型。</typeparam>
        /// <param name="handler">事件处理器。</param>
        void AddEventHandler<T>(DomainServiceEventHandler<T> handler) where T : DomainServiceEventArgs;

        /// <summary>
        /// 移除事件处理器。
        /// </summary>
        /// <typeparam name="T">事件参数类型。</typeparam>
        /// <param name="handler">事件处理器。</param>
        void RemoveEventHandler<T>(DomainServiceEventHandler<T> handler) where T : DomainServiceEventArgs;
        
        #endregion

        #region 引发事件

        /// <summary>
        /// 引发事件。
        /// </summary>
        /// <typeparam name="T">事件参数类型。</typeparam>
        /// <param name="context">领域执行上下文。</param>
        /// <param name="eventArgs">事件参数。</param>
        Task RaiseEvent<T>(IDomainExecutionContext context, T eventArgs) where T : DomainServiceEventArgs;

        #endregion
    }
}
