using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Metadata;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;
using System.Web.Security;

namespace System.Web.Mvc
{
    /// <summary>
    /// Entity authorize attribute base.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class EntityAuthorizeAttribute : FilterAttribute, IAuthorizationFilter
    {
        /// <summary>
        /// Initialize entity authorize attribute.
        /// </summary>
        public EntityAuthorizeAttribute()
            : this(EntityAuthorizeAction.None) { }

        /// <summary>
        /// Initialize entity authorize attribute.
        /// </summary>
        /// <param name="action">Action of entity authorize.</param>
        public EntityAuthorizeAttribute(EntityAuthorizeAction action)
        {
            Action = action;
        }

        /// <summary>
        /// Get the action of entity authorize.
        /// </summary>
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
        protected IEntityMetadata Metadata { get; private set; }

        /// <summary>
        /// When overridden, provides an entry point for custom authorization checks.
        /// </summary>
        /// <param name="httpContext">The HTTP context, which encapsulates all HTTP-specific information about an individual HTTP request.</param>
        /// <returns>true if the user is authorized; otherwise, false.</returns>
        protected virtual bool AuthorizeCore(HttpContextBase httpContext)
        {
            return httpContext.User.Identity.IsAuthenticated;
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

                    if (filterContext.RouteData.DataTokens["loginUrl"] == null)
                        filterContext.Result = new RedirectResult(ComBoostAuthentication.LoginUrl + "?returnUrl=" + Uri.EscapeDataString(filterContext.RequestContext.HttpContext.Request.Url.PathAndQuery));
                    else
                        filterContext.Result = new RedirectResult(filterContext.RouteData.DataTokens["loginUrl"].ToString() + "?returnUrl=" + Uri.EscapeDataString(filterContext.RequestContext.HttpContext.Request.Url.PathAndQuery));
                return;
            }
            if (filterContext.Controller is IHaveEntityMetadata)
                Metadata = ((IHaveEntityMetadata)filterContext.Controller).Metadata;
            RouteData = filterContext.RouteData;

            if (!Authorize(filterContext))
            {
                if (filterContext.HttpContext.User.Identity.IsAuthenticated)
                    filterContext.Result = new HttpUnauthorizedResult();
                else
                    if (filterContext.RouteData.DataTokens["loginUrl"] == null)
                        filterContext.Result = new RedirectResult(ComBoostAuthentication.LoginUrl + "?returnUrl=" + Uri.EscapeDataString(filterContext.RequestContext.HttpContext.Request.Url.PathAndQuery));
                    else
                        filterContext.Result = new RedirectResult(filterContext.RouteData.DataTokens["loginUrl"].ToString() + "?returnUrl=" + Uri.EscapeDataString(filterContext.RequestContext.HttpContext.Request.Url.PathAndQuery));
            }
        }
    }

    /// <summary>
    /// Entity authorize action
    /// </summary>
    public enum EntityAuthorizeAction
    {
        /// <summary>
        /// None.
        /// </summary>
        None = 0,
        /// <summary>
        /// View.
        /// </summary>
        View = 1,
        /// <summary>
        /// Create.
        /// </summary>
        Create = 2,
        /// <summary>
        /// Edit.
        /// </summary>
        Edit = 3,
        /// <summary>
        /// Remove
        /// </summary>
        Remove = 4
    }
}
