using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    /// <summary>
    /// 领域服务筛选器。
    /// </summary>
    public interface IDomainServiceFilter
    {
        /// <summary>
        /// 异步即将执行时。
        /// </summary>
        /// <param name="context">领域执行上下文。</param>
        /// <returns>异步任务。</returns>
        Task OnExecutingAsync(IDomainExecutionContext context);

        /// <summary>
        /// 异步执行完成时。
        /// </summary>
        /// <param name="context">领域执行上下文。</param>
        /// <returns>异步任务。</returns>
        Task OnExecutedAsync(IDomainExecutionContext context);

        /// <summary>
        /// 异步抛出异常时。
        /// </summary>
        /// <param name="context">领域执行上下文。</param>
        /// <param name="exception">异常内容。</param>
        /// <returns>异步任务。</returns>
        Task OnExceptionThrowingAsync(IDomainExecutionContext context, Exception exception);
    }
}
