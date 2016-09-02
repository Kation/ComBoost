using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        /// <param name="mode">Custom authorize mode.</param>
        /// <param name="roles">Custom authorize roles.</param>
        public EntityAuthorizeAttribute(AuthenticationRequiredMode mode, params object[] roles)
            : this(EntityAuthorizeAction.None)
        {
            CustomRolesRequiredMode = mode;
            CustomRoles = roles;
        }

        /// <summary>
        /// Initialize entity authorize attribute.
        /// </summary>
        /// <param name="action">Action of entity authorize.</param>
        public EntityAuthorizeAttribute(EntityAuthorizeAction action)
        {
            Action = action;
        }

        /// <summary>
        /// Get the custom authorize mode.
        /// </summary>
        public AuthenticationRequiredMode CustomRolesRequiredMode { get; private set; }

        /// <summary>
        /// Get the custom authorize roles.
        /// </summary>
        public object[] CustomRoles { get; private set; }

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
            if (Metadata != null)
            {
                if (!Metadata.AllowAnonymous && !httpContext.User.Identity.IsAuthenticated)
                    return false;
                switch (Action)
                {
                    case EntityAuthorizeAction.Create:
                        return Metadata.AddRoles.Count() == 0 ||
                            (Metadata.AuthenticationRequiredMode == ComponentModel.DataAnnotations.AuthenticationRequiredMode.All ?
                            Metadata.AddRoles.All(t => httpContext.User.IsInRole(t)) :
                            Metadata.AddRoles.Any(t => httpContext.User.IsInRole(t)));
                    case EntityAuthorizeAction.Edit:
                        return Metadata.EditRoles.Count() == 0 ||
                            (Metadata.AuthenticationRequiredMode == ComponentModel.DataAnnotations.AuthenticationRequiredMode.All ?
                            Metadata.EditRoles.All(t => httpContext.User.IsInRole(t)) :
                            Metadata.EditRoles.Any(t => httpContext.User.IsInRole(t)));
                    case EntityAuthorizeAction.Remove:
                        return Metadata.RemoveRoles.Count() == 0 ||
                            (Metadata.AuthenticationRequiredMode == ComponentModel.DataAnnotations.AuthenticationRequiredMode.All ?
                            Metadata.RemoveRoles.All(t => httpContext.User.IsInRole(t)) :
                            Metadata.RemoveRoles.Any(t => httpContext.User.IsInRole(t)));
                    case EntityAuthorizeAction.View:
                        return Metadata.ViewRoles.Count() == 0 ||
                            (Metadata.AuthenticationRequiredMode == ComponentModel.DataAnnotations.AuthenticationRequiredMode.All ?
                            Metadata.ViewRoles.All(t => httpContext.User.IsInRole(t)) :
                            Metadata.ViewRoles.Any(t => httpContext.User.IsInRole(t)));
                    case EntityAuthorizeAction.None:
                        return CustomRoles == null ||
                            (CustomRolesRequiredMode == ComponentModel.DataAnnotations.AuthenticationRequiredMode.All ?
                            CustomRoles.All(t => httpContext.User.IsInRole(t)) :
                            CustomRoles.Any(t => httpContext.User.IsInRole(t)));
                    default:
                        return false;
                }
            }
            else
                if (Action == EntityAuthorizeAction.None && CustomRoles != null)
                    return (CustomRolesRequiredMode == ComponentModel.DataAnnotations.AuthenticationRequiredMode.All ?
                            CustomRoles.All(t => httpContext.User.IsInRole(t)) :
                            CustomRoles.Any(t => httpContext.User.IsInRole(t)));
                else
                    return httpContext.User.Identity.IsAuthenticated;
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
                        filterContext.Result = new RedirectResult((filterContext.HttpContext.Request.ApplicationPath == "/" ? "" : filterContext.HttpContext.Request.ApplicationPath) + ComBoostAuthentication.LoginUrl + "?returnUrl=" + Uri.EscapeDataString(filterContext.RequestContext.HttpContext.Request.Url.PathAndQuery));
                    else
                        filterContext.Result = new RedirectResult((filterContext.HttpContext.Request.ApplicationPath == "/" ? "" : filterContext.HttpContext.Request.ApplicationPath) + filterContext.RouteData.DataTokens["loginUrl"].ToString() + "?returnUrl=" + Uri.EscapeDataString(filterContext.RequestContext.HttpContext.Request.Url.PathAndQuery));
                return;
            }
            if (filterContext.Controller is IHaveEntityMetadata)
                Metadata = ((IHaveEntityMetadata)filterContext.Controller).Metadata;
            RouteData = filterContext.RouteData;

            if (!AuthorizeCore(filterContext.RequestContext.HttpContext))
            {
                if (Action != EntityAuthorizeAction.None && filterContext.HttpContext.User.Identity.IsAuthenticated)
                    filterContext.Result = new HttpUnauthorizedResult();
                else
                    if (filterContext.RouteData.DataTokens["loginUrl"] == null)
                        filterContext.Result = new RedirectResult((filterContext.HttpContext.Request.ApplicationPath == "/" ? "" : filterContext.HttpContext.Request.ApplicationPath) + ComBoostAuthentication.LoginUrl + "?returnUrl=" + Uri.EscapeDataString(filterContext.RequestContext.HttpContext.Request.Url.PathAndQuery));
                    else
                        filterContext.Result = new RedirectResult((filterContext.HttpContext.Request.ApplicationPath == "/" ? "" : filterContext.HttpContext.Request.ApplicationPath) + filterContext.RouteData.DataTokens["loginUrl"].ToString() + "?returnUrl=" + Uri.EscapeDataString(filterContext.RequestContext.HttpContext.Request.Url.PathAndQuery));
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
