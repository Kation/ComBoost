using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity;

namespace Wodsoft.ComBoost.Mvc
{
    /// <summary>
    /// Mvc item button.
    /// Use for mvc viewlist page.
    /// </summary>
    public class EntityViewButton : ViewButton, IEntityViewButton
    {
        /// <summary>
        /// Get or set the link delegate.
        /// </summary>
        public MvcEntityItemButtonLinkDelegate GetLink { get; set; }
        
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
            var accessor = provider.GetService<Microsoft.AspNetCore.Mvc.Infrastructure.IActionContextAccessor>();
            var urlHelper = provider.GetService<Microsoft.AspNetCore.Mvc.Routing.IUrlHelperFactory>().GetUrlHelper(accessor.ActionContext);
            Target = GetLink(urlHelper, entity);
        }
        
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
    public delegate string MvcEntityItemButtonLinkDelegate(IUrlHelper url, IEntity entity);
}
