using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace System.ComponentModel
{
    /// <summary>
    /// Mvc view button.
    /// Use for mvc viewlist page.
    /// </summary>
    public class MvcViewButton : IViewButton
    {
        /// <summary>
        /// Get or set the name of button.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Get or set the icon url.
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// Get or set the tooltip.
        /// </summary>
        public string Tooltip { get; set; }

        /// <summary>
        /// Get or set the static link.
        /// </summary>
        public string StaticLink { get; set; }

        /// <summary>
        /// Get or set the link delegate.
        /// </summary>
        public MvcViewButtonLinkDelegate GetLink { get; set; }

        object IViewButton.Icon { get { return Icon; } }

        object IViewButton.Tooltip { get { return Tooltip; } }
        
        /// <summary>
        /// Set the target of button.
        /// </summary>
        /// <param name="provider">Service provider.</param>
        public void SetTarget(IServiceProvider provider)
        {
            if (GetLink == null)
                return;
            if (provider == null)
                throw new NotSupportedException();
            Controller controller = (Controller)provider.GetService(typeof(Controller));
            if (controller == null)
                throw new InvalidOperationException("Can not get controller from service provider.");
            StaticLink = GetLink(controller.Url);
        }

        /// <summary>
        /// Get the target of the button.
        /// </summary>
        public object Target { get { return StaticLink; } }
    }

    /// <summary>
    /// Mvc view button link delegate.
    /// </summary>
    /// <param name="url">Mvc url helper.</param>
    /// <returns>Return url link.</returns>
    public delegate string MvcViewButtonLinkDelegate(UrlHelper url);
}
