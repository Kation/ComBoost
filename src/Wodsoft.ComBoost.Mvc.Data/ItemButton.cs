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
    public class ItemButton : ViewButton, IItemButton
    {
        /// <summary>
        /// Get or set the link delegate.
        /// </summary>
        public MvcEntityItemButtonLinkDelegate? GetLink { get; set; }

        /// <inheritdoc />
        public void SetTarget(IServiceProvider provider, object item)
        {            
            if (GetLink == null)
                return;
            if (provider == null)
                throw new NotSupportedException();
            var accessor = provider.GetRequiredService<Microsoft.AspNetCore.Mvc.Infrastructure.IActionContextAccessor>();
            var urlHelper = provider.GetRequiredService<Microsoft.AspNetCore.Mvc.Routing.IUrlHelperFactory>().GetUrlHelper(accessor.ActionContext!);
            Target = GetLink(urlHelper, item);
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
    /// <param name="item">Dependency entity.</param>
    /// <returns>Return url link.</returns>
    public delegate string MvcEntityItemButtonLinkDelegate(IUrlHelper url, object item);
}
