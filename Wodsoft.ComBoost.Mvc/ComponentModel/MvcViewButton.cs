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
    public class MvcViewButton : EntityViewButton
    {
        /// <summary>
        /// Get or set the icon url.
        /// </summary>
        public new string Icon { get { return (string)base.Icon; } set { base.Icon = value; } }

        /// <summary>
        /// Get or set the static link.
        /// If StaticLink is not null, the button will always enable.
        /// </summary>
        public string StaticLink { get; set; }

        /// <summary>
        /// Get or set the link delegate.
        /// </summary>
        public MvcViewButtonLinkDelegate GetLink { get; set; }
    }

    /// <summary>
    /// Mvc view button link delegate.
    /// </summary>
    /// <param name="urlhelper">Mvc url helper.</param>
    /// <param name="entity">Entity for button context.</param>
    /// <returns>Return url link. Return null if button is disabled.</returns>
    public delegate string MvcViewButtonLinkDelegate(UrlHelper urlhelper, IEntity entity);
}
