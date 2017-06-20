using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    /// <summary>
    /// 领域服务筛选器特性。
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public abstract class DomainServiceFilterAttribute : Attribute, IDomainServiceFilter
    {
        /// <summary>
        /// 异步即将执行时。
        /// </summary>
        /// <param name="context">领域执行上下文。</param>
        /// <returns>异步任务。</returns>
        public virtual Task OnExecutingAsync(IDomainExecutionContext context)
        {
#if NET451
            return Task.FromResult(0);
#else
            return Task.CompletedTask;
#endif
        }

        /// <summary>
        /// 异步执行完成时。
        /// </summary>
        /// <param name="context">领域执行上下文。</param>
        /// <returns>异步任务。</returns>
        public virtual Task OnExecutedAsync(IDomainExecutionContext context)
        {
#if NET451
            return Task.FromResult(0);
#else
            return Task.CompletedTask;
#endif
        }

        /// <summary>
        /// 异步抛出异常时。
        /// </summary>
        /// <param name="context">领域执行上下文。</param>
        /// <param name="exception">异常内容。</param>
        /// <returns>异步任务。</returns>
        public Task OnExceptionThrowingAsync(IDomainExecutionContext context, Exception exception)
        {
#if NET451
            return Task.FromResult(0);
#else
            return Task.CompletedTask;
#endif
        }
    }
}
