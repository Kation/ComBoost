using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity;

namespace System.ComponentModel
{
    /// <summary>
    /// Entity viewlist button.
    /// </summary>
    public interface IEntityViewButton : IViewButton
    {
        /// <summary>
        /// Set the target of button.
        /// </summary>
        /// <param name="provider">Service provider.</param>
        /// <param name="entity">Dependency entity.</param>
        void SetTarget(IServiceProvider provider, IEntity entity);
    }
}
