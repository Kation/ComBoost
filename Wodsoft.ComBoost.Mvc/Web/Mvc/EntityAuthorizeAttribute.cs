using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Web.Mvc
{
    /// <summary>
    /// Entity authorize attribute base.
    /// </summary>
    public abstract class EntityAuthorizeAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// Get the context builder of entity.
        /// </summary>
        protected IEntityContextBuilder EntityBuilder { get; private set; }

        /// <summary>
        /// When overridden, provides an entry point for custom authorization checks.
        /// </summary>
        /// <param name="httpContext">The HTTP context, which encapsulates all HTTP-specific information about an individual HTTP request.</param>
        /// <returns>true if the user is authorized; otherwise, false.</returns>
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (!httpContext.User.Identity.IsAuthenticated)
                return false;
            EntityBuilder = httpContext.Items["EntityBuilder"] as IEntityContextBuilder;
            return true;
        }
    }
}
