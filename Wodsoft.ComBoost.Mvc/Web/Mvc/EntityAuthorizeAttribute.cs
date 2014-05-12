using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Metadata;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;

namespace System.Web.Mvc
{
    /// <summary>
    /// Entity authorize attribute base.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class EntityAuthorizeAttribute : FilterAttribute, IAuthorizationFilter
    {
        /// <summary>
        /// Initialize EntityAuthorizeAttribute with area General.
        /// </summary>
        public EntityAuthorizeAttribute() : this("General") { }
        
        /// <summary>
        /// Initialize EntityAuthorizeAttribute.
        /// </summary>
        /// <param name="authorizationArea">Authorization area. </param>
        public EntityAuthorizeAttribute(string authorizationArea)
        {
            if (authorizationArea == null)
                throw new ArgumentNullException("authorizationArea");
            AuthorizationArea = authorizationArea;
        }

        public string AuthorizationArea { get; private set; }

        /// <summary>
        /// Get the context builder of entity.
        /// </summary>
        protected IEntityContextBuilder EntityBuilder { get; private set; }

        /// <summary>
        /// Get the route data.
        /// </summary>
        protected RouteData RouteData { get; private set; }

        /// <summary>
        /// Get the entity metadata.
        /// </summary>
        protected EntityMetadata Metadata { get; private set; }

        /// <summary>
        /// When overridden, provides an entry point for custom authorization checks.
        /// </summary>
        /// <param name="httpContext">The HTTP context, which encapsulates all HTTP-specific information about an individual HTTP request.</param>
        /// <returns>true if the user is authorized; otherwise, false.</returns>
        protected virtual bool AuthorizeCore(HttpContextBase httpContext)
        {
            return true;
        }

        private bool Authorize(AuthorizationContext filterContext)
        {
            bool result = true;
            if (Metadata != null)
            {
                result &= (Metadata.AllowAnonymous || filterContext.HttpContext.User.Identity.IsAuthenticated);
                //if (filterContext.ActionDescriptor.ActionName == "Index")
                //{
                //    foreach (var role in Metadata.ViewRoles)
                //        if (!filterContext.HttpContext.User.IsInRole(role))
                //            return false;
                //}
                //else if (filterContext.ActionDescriptor.ActionName == "Index")
                //{
                //    foreach (var role in Metadata.ViewRoles)
                //        if (!filterContext.HttpContext.User.IsInRole(role))
                //            return false;
                //}
                //else if (filterContext.ActionDescriptor.ActionName == "Index")
                //{
                //    foreach (var role in Metadata.ViewRoles)
                //        if (!filterContext.HttpContext.User.IsInRole(role))
                //            return false;
                //}
            }
            if (result)
                result = AuthorizeCore(filterContext.HttpContext);
            return result;
        }

        /// <summary>
        /// Called when authorization is required.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext.Controller is EntityController)
                EntityBuilder = ((EntityController)filterContext.Controller).EntityBuilder;
            else
                return;
            if (filterContext.Controller.GetType().IsGenericType)
                Metadata = EntityAnalyzer.GetMetadata(filterContext.Controller.GetType().GetGenericArguments()[0]);
            RouteData = filterContext.RouteData;
            
            if (!Authorize(filterContext))
                filterContext.Result = new HttpStatusCodeResult(403);
        }
    }
}
