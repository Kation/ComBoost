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
    /// <summary>
    /// 权限过滤器。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class ComBoostAuthorizeAttribute : Attribute, IActionFilter
    {
        /// <summary>
        /// 实例化权限过滤器。用于过滤没有登录的用户。
        /// </summary>
        public ComBoostAuthorizeAttribute() : this(AuthenticationRequiredMode.All, Array.Empty<object>()) { }

        /// <summary>
        /// 实例化权限过滤器。用于过滤没有角色的用户。
        /// </summary>
        /// <param name="mode">认证模式。</param>
        /// <param name="roles">所需角色。</param>
        public ComBoostAuthorizeAttribute(AuthenticationRequiredMode mode, params object[] roles)
        {
            Mode = mode;
            Roles = roles;
        }

        /// <summary>
        /// 获取认证模式。
        /// </summary>
        public AuthenticationRequiredMode Mode { get; private set; }

        /// <summary>
        /// 获取角色。
        /// </summary>
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
                loginUrl += "&returnUrl=" + Uri.EscapeDataString(context.HttpContext.Request.Path);
            else
                loginUrl += "?returnUrl=" + Uri.EscapeDataString(context.HttpContext.Request.Path);
            return new RedirectResult(loginUrl);
        }
    }
}
