using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Security
{
    public class ComBoostAuthenticationHandler : AuthenticationHandler<ComBoostAuthenticationOptions>
    {
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            return Task.FromResult(AuthenticateResult.Skip());
        }
    }
}
