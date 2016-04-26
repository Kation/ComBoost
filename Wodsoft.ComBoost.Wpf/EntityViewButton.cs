using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Wodsoft.ComBoost.Wpf;

namespace Wodsoft.ComBoost.Wpf
{
    public class EntityViewButton : IViewButton
    {
        public EntityViewButtonCommandDelegate GetInvokeDelegate { get; set; }

        public Task InvokeDelegate { get; private set; }

        public string Name { get; set; }

        public object Tooltip { get; set; }

        public object Icon { get; set; }

        public object Target { get { return InvokeDelegate; } }

        public void SetTarget(IServiceProvider provider)
        {
            EntityViewer viewer = (EntityViewer)provider.GetService(typeof(EntityViewer));
            InvokeDelegate = GetInvokeDelegate(viewer);
        }
    }

    /// <summary>
    /// Entity view button invoke delegate.
    /// </summary>
    /// <param name="viewer">Entity viewer.</param>
    /// <returns>Return invoke delegate.</returns>
    public delegate Task EntityViewButtonCommandDelegate(EntityViewer viewer);
}
