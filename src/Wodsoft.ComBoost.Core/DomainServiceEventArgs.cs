using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost
{
    /// <summary>
    /// 领域服务事件参数。
    /// </summary>
    [Serializable]
    public class DomainServiceEventArgs : EventArgs
    {
        /// <summary>
        /// 获取或设置是否已处理。
        /// 已处理会中断事件路由。
        /// </summary>
        public bool IsHandled { get; set; }
    }
}
