using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ComponentModel
{
    /// <summary>
    /// View button interface.
    /// </summary>
    public interface IViewButton
    {
        /// <summary>
        /// Get the name of the button.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Get the tooltip of the button.
        /// </summary>
        object Tooltip { get; }

        /// <summary>
        /// Get the icon of the button.
        /// </summary>
        object Icon { get; }

        /// <summary>
        /// Get the target of the button.
        /// </summary>
        object Target { get; }

        /// <summary>
        /// Set the target of button.
        /// </summary>
        /// <param name="provider">Service provider.</param>
        void SetTarget(IServiceProvider provider);
    }
}
