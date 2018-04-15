using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    /// <summary>
    /// 领域扩展。
    /// </summary>
    public interface IDomainExtension
    {
        /// <summary>
        /// 领域初始化。
        /// </summary>
        /// <param name="serviceProvider">服务提供器。</param>
        /// <param name="domainService">要初始化的领域服务。</param>
        void OnInitialize(IServiceProvider serviceProvider, IDomainService domainService);

        /// <summary>
        /// 领域执行前调用方法。
        /// </summary>
        /// <param name="context">领域执行上下文。</param>
        /// <param name="e">事件参数。</param>
        /// <returns>返回异步任务。</returns>
        Task OnExecutingAsync(IDomainExecutionContext context, DomainServiceEventArgs e);

        /// <summary>
        /// 领域执行后调用方法。
        /// </summary>
        /// <param name="context">领域执行上下文。</param>
        /// <param name="e">事件参数。</param>
        /// <returns>返回异步任务。</returns>
        Task OnExecutedAsync(IDomainExecutionContext context, DomainServiceEventArgs e);
    }
}
