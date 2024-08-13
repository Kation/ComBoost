using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Security;

namespace Wodsoft.ComBoost.Mock
{
    public class AnonymousAuthenticationHandler : IAuthenticationHandler
    {
        public Task<AuthenticationResult> AuthenticateAsync()
        {
            ClaimsIdentity identity = new ClaimsIdentity("Anonymous");
            var user = new ClaimsPrincipal(identity);
            return Task.FromResult(AuthenticationResult.Success(user));
        }
    }
}
