using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Wodsoft.ComBoost.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.Features.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace Wodsoft.ComBoost.Mvc
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ComBoostAuthorizeAttribute : Attribute, IActionFilter
    {
        public ComBoostAuthorizeAttribute()
            : this(AuthenticationRequiredMode.All, new object[0])
        { }

        public ComBoostAuthorizeAttribute(params object[] roles) : this(AuthenticationRequiredMode.All, roles) { }

        public ComBoostAuthorizeAttribute(AuthenticationRequiredMode mode, params object[] roles)
        {
            Mode = mode;
            Roles = roles;
        }

        public AuthenticationRequiredMode Mode { get; private set; }

        public object[] Roles { get; private set; }

        public virtual void OnActionExecuted(ActionExecutedContext context) { }

        public virtual void OnActionExecuting(ActionExecutingContext context)
        {
            bool isAuthed = true;
            if (!context.HttpContext.User.Identity.IsAuthenticated)
            {
                isAuthed = false;
            }
            else if (Mode == AuthenticationRequiredMode.All ?
                !Roles.All(t => context.HttpContext.User.IsInDynamicRole(t)) :
                !Roles.Any(t => context.HttpContext.User.IsInDynamicRole(t)))
            {
                isAuthed = false;
            }
            if (!isAuthed)
            {
                var options = context.HttpContext.RequestServices.GetRequiredService<ComBoostAuthenticationOptions>();
                var loginUrl = options.LoginPath(context.HttpContext);
                loginUrl = context.HttpContext.Request.PathBase.Add(loginUrl) + "?returlUrl=" + Uri.EscapeDataString(context.HttpContext.Request.Path);
                context.Result = new RedirectResult(loginUrl);
            }
        }
    }
}
