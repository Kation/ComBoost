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
    public class ComBoostAuthenticationCookieHandler : AuthenticationHandler<ComBoostAuthenticationOptions>
    {
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            string cookieValue = Request.Cookies[Options.CookieName(Context)];
            if (cookieValue == null)
                return Task.FromResult(AuthenticateResult.Skip());
            try
            {
                var ticket = Options.TicketDataFormat.Unprotect(cookieValue, GetTlsTokenBinding());
                return Task.FromResult(AuthenticateResult.Success(ticket));
            }
            catch (Exception ex)
            {
                return Task.FromResult(AuthenticateResult.Fail(ex));
            }
        }

        protected override Task HandleSignInAsync(SignInContext context)
        {
            var ticket = new AuthenticationTicket(context.Principal, null, Options.AuthenticationScheme);
            var cookieValue = Options.TicketDataFormat.Protect(ticket, GetTlsTokenBinding());

            Response.Cookies.Append(Options.CookieName(Context), cookieValue, new CookieOptions
            {
                Domain = Options.CookieDomain(Context),
                Expires = new DateTimeOffset(DateTime.Now.Add(Options.ExpireTime(Context))),
                Path = Options.CookiePath(Context)
            });
            return Task.FromResult(0);
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

        protected virtual string GetTlsTokenBinding()
        {
            var binding = Context.Features.Get<ITlsTokenBindingFeature>()?.GetProvidedTokenBindingId();
            return binding == null ? null : Convert.ToBase64String(binding);
        }
    }
}
