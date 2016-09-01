using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using System.ComponentModel.DataAnnotations;
using Wodsoft.ComBoost.Security;
using Microsoft.AspNetCore.Mvc;

namespace Wodsoft.ComBoost.Mvc
{
    public class EntityAuthorizeAttribute : IActionFilter
    {
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

        public virtual void OnActionExecuted(ActionExecutedContext context) { }
        
        public virtual void OnActionExecuting(ActionExecutingContext context)
        {
            var controller = context.Controller as IHaveEntityMetadata;
            if (controller == null)
                return;
            
            bool isAuthed = true;
            switch (Action)
            {
                case EntityAuthorizeAction.Create:
                    isAuthed= controller.Metadata.AddRoles.Count() == 0 ||
                        (controller.Metadata.AuthenticationRequiredMode == AuthenticationRequiredMode.All ?
                        controller.Metadata.AddRoles.All(t => context.HttpContext.User.IsInDynamicRole(t)) :
                        controller.Metadata.AddRoles.Any(t => context.HttpContext.User.IsInDynamicRole(t)));
                    break;
                case EntityAuthorizeAction.Edit:
                    isAuthed = controller.Metadata.EditRoles.Count() == 0 ||
                        (controller.Metadata.AuthenticationRequiredMode == AuthenticationRequiredMode.All ?
                        controller.Metadata.EditRoles.All(t => context.HttpContext.User.IsInDynamicRole(t)) :
                        controller.Metadata.EditRoles.Any(t => context.HttpContext.User.IsInDynamicRole(t)));
                    break;
                case EntityAuthorizeAction.Remove:
                    isAuthed = controller.Metadata.RemoveRoles.Count() == 0 ||
                        (controller.Metadata.AuthenticationRequiredMode == AuthenticationRequiredMode.All ?
                        controller.Metadata.RemoveRoles.All(t => context.HttpContext.User.IsInDynamicRole(t)) :
                        controller.Metadata.RemoveRoles.Any(t => context.HttpContext.User.IsInDynamicRole(t)));
                    break;
                case EntityAuthorizeAction.View:
                    isAuthed = controller.Metadata.ViewRoles.Count() == 0 ||
                        (controller.Metadata.AuthenticationRequiredMode == AuthenticationRequiredMode.All ?
                        controller.Metadata.ViewRoles.All(t => context.HttpContext.User.IsInDynamicRole(t)) :
                        controller.Metadata.ViewRoles.Any(t => context.HttpContext.User.IsInDynamicRole(t)));
                    break;
                default:
                    isAuthed = false;
                    break;
            }
            if (!isAuthed)
            {
                var middleware = context.HttpContext.Features.Get<ComBoostAuthenticationMiddleware>();
                var loginUrl = middleware.Options.LoginPath(context.HttpContext);
                loginUrl = context.HttpContext.Request.PathBase.Add(loginUrl + "?returlUrl=" + Uri.EscapeDataString(context.HttpContext.Request.Path));
                context.Result = new RedirectResult(loginUrl);
            }
        }
    }

    /// <summary>
    /// Entity authorize action
    /// </summary>
    public enum EntityAuthorizeAction
    {
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
