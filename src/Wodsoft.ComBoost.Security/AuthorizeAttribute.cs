using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Security;

namespace Wodsoft.ComBoost
{
    /// <summary>
    /// 权限过滤器。
    /// </summary>
    public class AuthorizeAttribute : DomainServiceFilterAttribute
    {
        /// <summary>
        /// 空权限。需要登录。
        /// </summary>
        public AuthorizeAttribute()
        {
            Roles = new object[0];
        }

        /// <summary>
        /// 指定角色。
        /// </summary>
        /// <param name="mode">判断模式。</param>
        /// <param name="roles">角色。</param>
        public AuthorizeAttribute(AuthenticationRequiredMode mode, params object[] roles)
        {
            Mode = mode;
            Roles = roles;
        }

        /// <summary>
        /// 获取权限判断模式。
        /// </summary>
        public AuthenticationRequiredMode Mode { get; private set; }

        /// <summary>
        /// 获取需要的角色。
        /// </summary>
        public object[] Roles { get; private set; }

        /// <inheritdoc/>
        public override Task OnExecutingAsync(IDomainExecutionContext context)
        {
            var provider = context.DomainContext.GetService<IAuthenticationProvider>();
            var authentication = provider.GetAuthentication();
            if (!authentication.Identity.IsAuthenticated)
                throw new UnauthorizedAccessException("用户未登录。");
            if (Roles.Length > 0)
                if (Mode == AuthenticationRequiredMode.All)
                {
                    foreach (var role in Roles)
                        if (!authentication.IsInRole(role))
                            throw new UnauthorizedAccessException("用户没有“" + role + "”的权限。");
                }
                else
                {
                    foreach (var role in Roles)
                        if (authentication.IsInRole(role))
                            return Task.FromResult(0);
                    throw new UnauthorizedAccessException("用户没有" + string.Join("，", "“" + Roles + "”") + "权限。");
                }
            return Task.FromResult(0);
        }
    }
}
