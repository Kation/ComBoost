using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;

namespace System.Web.Mvc
{
    /// <summary>
    /// Entity authorize attribute base.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public abstract class EntityAuthorizeAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// Get the context builder of entity.
        /// </summary>
        protected IEntityContextBuilder EntityBuilder { get; private set; }

        /// <summary>
        /// Provides an entry point for custom authorization checks.
        /// </summary>
        /// <param name="httpContext">The HTTP context, which encapsulates all HTTP-specific information about an individual HTTP request.</param>
        /// <param name="routeData">Information about route.</param>
        /// <returns>true if the user is authorized; otherwise, false.</returns>
        protected abstract bool Authorize(HttpContextBase httpContext, RouteData routeData);

        /// <summary>
        /// Provides an entry point for custom authorization checks.
        /// </summary>
        /// <param name="httpContext">The HTTP context, which encapsulates all HTTP-specific information about an individual HTTP request.</param>
        /// <returns>true if the user is authorized; otherwise, false.</returns>
        protected sealed override bool AuthorizeCore(HttpContextBase httpContext)
        {
            EntityBuilder = httpContext.Items["EntityBuilder"] as IEntityContextBuilder;
            RouteData routeData = httpContext.Items["RouteData"] as RouteData;
            return Authorize(httpContext, routeData);
        }
    }
}
