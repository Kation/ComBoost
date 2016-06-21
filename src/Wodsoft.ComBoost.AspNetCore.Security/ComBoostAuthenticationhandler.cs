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
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http;

namespace Wodsoft.ComBoost.Security
{
    public class ComBoostAuthenticationHandler : AuthenticationHandler<ComBoostAuthenticationOptions>
    {
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            string cookieValue = Request.Cookies[Options.CookieName(Context)];
            if (cookieValue == null)
                return Task.FromResult(AuthenticateResult.Skip());
            var ticket = Options.TicketDataFormat.Unprotect(cookieValue);
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }

        protected override async Task HandleSignInAsync(SignInContext context)
        {
            var securityProvider = Context.RequestServices.GetRequiredService<ISecurityProvider>();
            var permission = await securityProvider.GetPermissionAsync(context.Properties);
            ClaimsPrincipal principal = new ClaimsPrincipal();
            ClaimsIdentity identity = new ClaimsIdentity("ComBoostAuthentication", ClaimTypes.Name, ClaimTypes.Role);
            identity.AddClaims(permission.GetStaticRoles().Select(t => new Claim(ClaimTypes.Role, securityProvider.ConvertRoleToString(t))));
            identity.AddClaim(new Claim(ClaimTypes.Name, permission.Name));
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, permission.Identity));
            principal.AddIdentity(identity);

            var ticket = new AuthenticationTicket(principal, null, Options.AuthenticationScheme);
            var cookieValue = Options.TicketDataFormat.Protect(ticket, GetTlsTokenBinding());

            Response.Cookies.Append(Options.CookieName(Context), cookieValue, new CookieOptions
            {
                Domain = Options.CookieDomain(Context),
                Expires = new DateTimeOffset(DateTime.Now, Options.ExpireTime(Context)),
                Path = Options.CookiePath(Context)
            });
        }

        protected override Task HandleSignOutAsync(SignOutContext context)
        {
            Response.Cookies.Delete(Options.CookieName(Context), new CookieOptions
            {
                Domain = Options.CookieDomain(Context),
                Path = Options.CookiePath(Context)
            });
            return Task.FromResult(0);
        }

        private string GetTlsTokenBinding()
        {
            var binding = Context.Features.Get<ITlsTokenBindingFeature>()?.GetProvidedTokenBindingId();
            return binding == null ? null : Convert.ToBase64String(binding);
        }
    }
}
