using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    /// <summary>
    /// 领域服务异步事件模式。
    /// </summary>
    public enum DomainServiceAsyncEventMode
    {
        /// <summary>
        /// 等待事件执行完毕。
        /// </summary>
        Await = 0,
        /// <summary>
        /// 异步执行事件不等待。
        /// </summary>
        Async = 1
    }
}
