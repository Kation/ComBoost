using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity;

namespace System.ComponentModel
{
    /// <summary>
    /// 内容项按钮。
    /// </summary>
    public interface IItemButton : IViewButton
    {
        /// <summary>
        /// 设置目标按钮。
        /// </summary>
        /// <param name="provider">服务提供器。</param>
        /// <param name="item">目标项。</param>
        void SetTarget(IServiceProvider provider, object item);
    }
}
