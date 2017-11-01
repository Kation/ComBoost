using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Wodsoft.ComBoost.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.Features.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Wodsoft.ComBoost.Mvc
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class ComBoostAuthorizeAttribute : Attribute, IActionFilter
    {
        public ComBoostAuthorizeAttribute() : this(AuthenticationRequiredMode.All, Array.Empty<object>()) { }

        public ComBoostAuthorizeAttribute(AuthenticationRequiredMode mode, params object[] roles)
        {
            Mode = mode;
            Roles = roles;
        }

        public AuthenticationRequiredMode Mode { get; private set; }

        public object[] Roles { get; private set; }

        public virtual void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Result is StatusCodeResult && ((StatusCodeResult)context.Result).StatusCode == 401 && !context.HttpContext.User.Identity.IsAuthenticated)
                context.Result = GetLoginUrl(context);
            else if (context.Result is ObjectResult && ((ObjectResult)context.Result).StatusCode == 401 && !context.HttpContext.User.Identity.IsAuthenticated)
                context.Result = GetLoginUrl(context);
        }

        public virtual void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.Filters.Count(t => t is ComBoostAnonymousFilter) > 0)
                return;
            if (!AuthorizeCore(context, context.Controller))
                context.Result = GetLoginUrl(context);
        }

        protected virtual bool AuthorizeCore(FilterContext context, object controller)
        {
            if (!context.HttpContext.User.Identity.IsAuthenticated)
                return false;
            if (Roles.Length == 0)
                return true;
            var authenticationProvider = context.HttpContext.RequestServices.GetRequiredService<IAuthenticationProvider>();
            var authentication = authenticationProvider.GetAuthentication();
            if (Mode == AuthenticationRequiredMode.All)
                return Roles.All(t => authentication.IsInRole(t));
            else
                return Roles.Any(t => authentication.IsInRole(t));
        }

        protected virtual RedirectResult GetLoginUrl(FilterContext context)
        {
            var options = context.HttpContext.RequestServices.GetRequiredService<IOptionsMonitor<ComBoostAuthenticationOptions>>().Get("ComBoost");
            var loginUrl = options.LoginPath(context.HttpContext);
            loginUrl = context.HttpContext.Request.PathBase.Add(loginUrl);
            if (loginUrl.Contains("?"))
                loginUrl += "&returlUrl=" + Uri.EscapeDataString(context.HttpContext.Request.Path);
            else
                loginUrl += "?returlUrl=" + Uri.EscapeDataString(context.HttpContext.Request.Path);
            return new RedirectResult(loginUrl);
        }
    }
}
