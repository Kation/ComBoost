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
        /// Set the target of button.
        /// </summary>
        /// <param name="provider">Service provider.</param>
        /// <param name="item">Dependency entity.</param>
        void SetTarget(IServiceProvider provider, object item);
    }
}
