using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    /// <summary>
    /// 领域服务过滤器。
    /// </summary>
    public interface IDomainServiceFilter
    {
        /// <summary>
        /// 异步执行。
        /// </summary>
        /// <param name="context">领域执行上下文。</param>
        /// <param name="next">下一个执行委托。</param>
        /// <returns></returns>
        Task OnExecutionAsync(IDomainExecutionContext context, DomainExecutionPipeline next);
    }
}
