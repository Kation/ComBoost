using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Security;

namespace Wodsoft.ComBoost
{
    public class AuthorizeAttribute : DomainServiceFilterAttribute
    {
        public AuthorizeAttribute()
        {
            Roles = new object[0];
        }

        public AuthorizeAttribute(AuthenticationRequiredMode mode, params object[] roles)
        {
            Mode = mode;
            Roles = roles;
        }

        public AuthenticationRequiredMode Mode { get; private set; }

        public object[] Roles { get; private set; }

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
