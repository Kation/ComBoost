using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    /// <summary>
    /// 领域服务事件委托。
    /// </summary>
    /// <typeparam name="T">事件参数类型。</typeparam>
    /// <param name="context">领域执行上下文。</param>
    /// <param name="e">事件参数。</param>
    public delegate Task DomainServiceEventHandler<T>(IDomainExecutionContext context, T e) where T : DomainServiceEventArgs;
}
