using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Wpf
{
    public class EntityItemButton : IEntityViewButton
    {
        public EntityItemButtonCommandDelegate GetInvokeDelegate { get; set; }

        public void SetTarget(IServiceProvider provider, IEntity entity)
        {
            EntityViewer viewer = (EntityViewer)provider.GetService(typeof(EntityViewer));
            InvokeDelegate = GetInvokeDelegate(viewer, entity);
        }

        public string Name { get; set; }

        public object Tooltip { get; set; }

        public object Icon { get; set; }
        
        public Task InvokeDelegate { get; private set; }

        public object Target
        {
            get { return InvokeDelegate; }
        }

        void IViewButton.SetTarget(IServiceProvider provider)
        {
            throw new NotSupportedException();
        }
    }

    /// <summary>
    /// Entity item button invkoe delegate.
    /// </summary>
    /// <param name="viewer">Entity viewer.</param>
    /// <param name="entity">Entity.</param>
    /// <returns>Return invoke delegate.</returns>
    public delegate Task EntityItemButtonCommandDelegate(EntityViewer viewer, IEntity entity);
}
