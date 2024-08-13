using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Security;

namespace Wodsoft.ComBoost.AspNetCore
{
    public class AspNetCoreAuthenticationHandler : IAuthenticationHandler
    {
        public AspNetCoreAuthenticationHandler(IHttpContextAccessor accessor)
        {
            if (accessor == null)
                throw new ArgumentNullException(nameof(accessor));
            Context = accessor.HttpContext;
        }

        public HttpContext Context { get; }

        public Task<AuthenticationResult> AuthenticateAsync()
        {
            if (Context == null)
                return Task.FromResult(AuthenticationResult.Fail());
            return Task.FromResult(AuthenticationResult.Success(Context.User));
        }
    }
}
