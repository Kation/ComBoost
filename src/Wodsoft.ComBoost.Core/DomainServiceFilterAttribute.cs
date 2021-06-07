using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    /// <summary>
    /// 领域服务过滤器特性。
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public abstract class DomainServiceFilterAttribute : Attribute, IDomainServiceFilter
    {
        /// <summary>
        /// 异步执行。
        /// </summary>
        /// <param name="context">领域执行上下文。</param>
        /// <returns>异步任务。</returns>
        public virtual Task OnExecutionAsync(IDomainExecutionContext context, DomainExecutionPipeline next)
        {
            return next();
        }
    }
}
