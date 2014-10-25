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
        public EntityAuthorizeAttribute()
            : this(EntityAuthorizeAction.None) { }

        public EntityAuthorizeAttribute(EntityAuthorizeAction action)
        {
            Action = action;
        }

        public EntityAuthorizeAction Action { get; private set; }

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
            if (Metadata != null)
            {
                if (!Metadata.AllowAnonymous && !filterContext.RequestContext.HttpContext.User.Identity.IsAuthenticated)
                    return false;
                switch (Action)
                {
                    case EntityAuthorizeAction.Create:
                        return Metadata.AddRoles.All(t => filterContext.RequestContext.HttpContext.User.IsInRole(t));
                    case EntityAuthorizeAction.Edit:
                        return Metadata.EditRoles.All(t => filterContext.RequestContext.HttpContext.User.IsInRole(t));
                    case EntityAuthorizeAction.Remove:
                        return Metadata.RemoveRoles.All(t => filterContext.RequestContext.HttpContext.User.IsInRole(t));
                    case EntityAuthorizeAction.View:
                        return Metadata.ViewRoles.All(t => filterContext.RequestContext.HttpContext.User.IsInRole(t));
                    case EntityAuthorizeAction.None:
                        return true;
                    default:
                        return false;
                }
            }
            else
                return AuthorizeCore(filterContext.RequestContext.HttpContext);
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
            {
                if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
                    filterContext.Result = new RedirectResult(System.Web.Security.ComBoostAuthentication.LoginUrl);
                return;
            }
            if (filterContext.Controller is IEntityMetadata)
                Metadata = ((IEntityMetadata)filterContext.Controller).Metadata;
            RouteData = filterContext.RouteData;

            if (!Authorize(filterContext))
            {
                if (filterContext.HttpContext.User.Identity.IsAuthenticated)
                    filterContext.Result = new HttpUnauthorizedResult();
                else
                    if (filterContext.RouteData.DataTokens["loginUrl"] == null)
                        filterContext.Result = new RedirectResult(System.Web.Security.ComBoostAuthentication.LoginUrl + "?returnUrl=" + Uri.EscapeDataString(filterContext.RequestContext.HttpContext.Request.Url.PathAndQuery));
                    else
                        filterContext.Result = new RedirectResult(filterContext.RouteData.DataTokens["loginUrl"].ToString() + "?returnUrl=" + Uri.EscapeDataString(filterContext.RequestContext.HttpContext.Request.Url.PathAndQuery));
            }
        }
    }

    public enum EntityAuthorizeAction
    {
        None = 0,
        View = 1,
        Create = 2,
        Edit = 3,
        Remove = 4
    }
}
