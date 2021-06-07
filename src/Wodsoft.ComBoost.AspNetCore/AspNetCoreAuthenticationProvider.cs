using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Security;

namespace Wodsoft.ComBoost.AspNetCore
{
    public class AspNetCoreAuthenticationProvider : IAuthenticationProvider
    {
        public AspNetCoreAuthenticationProvider(IHttpContextAccessor accessor)
        {
            if (accessor == null)
                throw new ArgumentNullException(nameof(accessor));
            if (accessor.HttpContext == null)
                throw new ArgumentException("当前环境不存在Http上下文。", "accessor");
            Context = accessor.HttpContext;
        }

        public HttpContext Context { get; }

        public ClaimsPrincipal User => Context.User;

        public bool IsInRole(string role)
        {
            return User.IsInRole(role);
        }
    }
}
