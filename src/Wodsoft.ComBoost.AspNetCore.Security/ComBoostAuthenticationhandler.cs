using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Features.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace Wodsoft.ComBoost.Security
{
    public class ComBoostAuthenticationHandler : AuthenticationHandler<ComBoostAuthenticationOptions>
    {
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            
            var identity = new ClaimsIdentity("Comboost", ClaimTypes.Name, "Test");
            identity.AddClaim(new Claim("Test", "1"));
            identity.AddClaim(new Claim("Test", "2"));
            identity.AddClaim(new Claim("Test", "3"));
            identity.AddClaim(new Claim(ClaimTypes.Name, "Kation"));
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()));
            var principal = new ClaimsPrincipal();
            principal.AddIdentity(identity);
            var ticket = new AuthenticationTicket(principal, new AuthenticationProperties()
            {
                IsPersistent = true
            }, "ComBoost");
            return Task.FromResult(AuthenticateResult.Success(ticket));
            //return Task.FromResult(AuthenticateResult.Skip());
            //Context.Authentication.SignInAsync
        }

        protected override Task HandleSignInAsync(SignInContext context)
        {
            var securityProvider = Context.RequestServices.GetRequiredService<ISecurityProvider>();
            securityProvider.GetPermission(context.Properties);
            return base.HandleSignInAsync(context);
        }
    }
}
