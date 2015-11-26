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
    /// Mvc item button.
    /// Use for mvc viewlist page.
    /// </summary>
    public class MvcEntityViewButton : IEntityViewButton
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
        /// Get or set the button link.
        /// If value is not null, the button will be disabled.
        /// </summary>
        public string Link { get; set; }

        /// <summary>
        /// Get or set the link delegate.
        /// </summary>
        public MvcEntityViewButtonLinkDelegate GetLink { get; set; }

        object IViewButton.Icon { get { return Icon; } }

        object IViewButton.Tooltip { get { return Tooltip; } }

        /// <summary>
        /// Set the target of button.
        /// </summary>
        /// <param name="provider">Service provider.</param>
        /// <param name="entity">Dependency entity.</param>
        public void SetTarget(IServiceProvider provider, IEntity entity)
        {
            if (GetLink == null)
                return;
            if (provider == null)
                throw new NotSupportedException();
            Controller controller = (Controller)provider.GetService(typeof(Controller));
            if (controller == null)
                throw new InvalidOperationException("Can not get controller from service provider.");
            Link = GetLink(controller.Url, entity);
        }

        /// <summary>
        /// Get the target of the button.
        /// </summary>
        public object Target { get { return Link; } }
        
        void IViewButton.SetTarget(IServiceProvider provider)
        {
            throw new NotSupportedException();
        }
    }

    /// <summary>
    /// Mvc entity button link delegate.
    /// </summary>
    /// <param name="url">Mvc url helper.</param>
    /// <param name="entity">Dependency entity.</param>
    /// <returns>Return url link.</returns>
    public delegate string MvcEntityViewButtonLinkDelegate(UrlHelper url, IEntity entity);
}
