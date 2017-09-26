using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Features.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;

namespace Wodsoft.ComBoost.Security
{
    public class ComBoostAuthenticationCookieHandler : AuthenticationHandler<ComBoostAuthenticationOptions>,
        IAuthenticationSignInHandler,
        IAuthenticationSignOutHandler
    {
        public ComBoostAuthenticationCookieHandler(IOptionsMonitor<ComBoostAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) 
            : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            string cookieValue = Request.Cookies[Options.CookieName(Context)];
            if (cookieValue == null)
                return Task.FromResult(AuthenticateResult.NoResult());
            try
            {
                var ticket = Options.TicketDataFormat.Unprotect(cookieValue, GetTlsTokenBinding());
                if (ticket.Properties.ExpiresUtc < DateTimeOffset.Now)
                    return Task.FromResult(AuthenticateResult.NoResult());
                if (Options.AutoUpdate(Context))
                {
                    var expireTime = Options.ExpireTime(Context);
                    var expireDate = expireTime.HasValue ? (DateTimeOffset?)DateTimeOffset.Now.Add(expireTime.Value) : null;
                    ticket.Properties.ExpiresUtc = expireDate;
                    cookieValue = Options.TicketDataFormat.Protect(ticket, GetTlsTokenBinding());
                    Response.Cookies.Append(Options.CookieName(Context), cookieValue, new CookieOptions
                    {
                        Domain = Options.CookieDomain(Context),
                        Expires = expireDate,
                        Path = Options.CookiePath(Context)
                    });
                }
                return Task.FromResult(AuthenticateResult.Success(ticket));
            }
            catch (Exception ex)
            {
                return Task.FromResult(AuthenticateResult.Fail(ex));
            }
        }

        public virtual Task SignInAsync(ClaimsPrincipal user, AuthenticationProperties properties)
        {
            var expireTime = Options.ExpireTime(Context);
            var expireDate = expireTime.HasValue ? (DateTimeOffset?)DateTimeOffset.Now.Add(expireTime.Value) : null;

            var ticket = new AuthenticationTicket(user, new AuthenticationProperties() { ExpiresUtc = expireDate }, "ComBoost");
            var cookieValue = Options.TicketDataFormat.Protect(ticket, GetTlsTokenBinding());

            Response.Cookies.Append(Options.CookieName(Context), cookieValue, new CookieOptions
            {
                Domain = Options.CookieDomain(Context),
                Expires = expireDate,
                Path = Options.CookiePath(Context)
            });
            return Task.FromResult(0);
        }

        public virtual Task SignOutAsync(AuthenticationProperties properties)
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
