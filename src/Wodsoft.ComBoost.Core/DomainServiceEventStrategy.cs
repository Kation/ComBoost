using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost
{
    /// <summary>
    /// 领域服务事件策略。
    /// </summary>
    public enum DomainServiceEventStrategy
    {
        /// <summary>
        /// 冒泡策略。
        /// </summary>
        Bubble = 0,
        /// <summary>
        /// 无策略。
        /// </summary>
        Direct = 1,
        /// <summary>
        /// 隧道策略。
        /// </summary>
        Tunnel = 2,
    }
}
