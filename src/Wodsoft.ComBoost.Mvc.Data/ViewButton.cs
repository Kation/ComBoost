using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Mvc
{
    public class ViewButton : IViewButton
    {
        /// <summary>
        /// Get or set the name of button.
        /// </summary>
        public virtual string? Name { get; set; }

        /// <summary>
        /// Get or set the icon url.
        /// </summary>
        public virtual string? Icon { get; set; }

        /// <summary>
        /// Get or set the tooltip.
        /// </summary>
        public virtual string? Tooltip { get; set; }

        object? IViewButton.Icon { get { return Icon; } }

        object? IViewButton.Tooltip { get { return Tooltip; } }
        
        /// <summary>
        /// Get or set the target of the button.
        /// </summary>
        public virtual object? Target { get; set; }

        public void SetTarget(IServiceProvider provider)
        {
            throw new NotSupportedException();
        }
    }

    public delegate string MvcEntityViewButtonLinkDelegate(IUrlHelper url);
}
