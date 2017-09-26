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
    public class ComBoostAuthenticationSessionHandler : AuthenticationHandler<ComBoostAuthenticationOptions>,
        IAuthenticationSignInHandler,
        IAuthenticationSignOutHandler
    {
        public ComBoostAuthenticationSessionHandler(IOptionsMonitor<ComBoostAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var ticketData = Context.Session.GetString(Options.CookieName(Context));
            if (ticketData == null)
                return Task.FromResult(AuthenticateResult.NoResult());
            try
            {
                var ticket = Options.TicketDataFormat.Unprotect(ticketData, GetTlsTokenBinding());
                if (ticket.Properties.ExpiresUtc < DateTimeOffset.Now)
                    return Task.FromResult(AuthenticateResult.NoResult());
                if (Options.AutoUpdate(Context))
                {
                    var expireTime = Options.ExpireTime(Context);
                    var expireDate = expireTime.HasValue ? (DateTimeOffset?)DateTimeOffset.Now.Add(expireTime.Value) : null;
                    ticket.Properties.ExpiresUtc = expireDate;
                    ticketData = Options.TicketDataFormat.Protect(ticket, GetTlsTokenBinding());
                    Context.Session.SetString(Options.CookieName(Context), ticketData);
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
            var ticketValue = Options.TicketDataFormat.Protect(ticket, GetTlsTokenBinding());
            Context.Session.SetString(Options.CookieName(Context), ticketValue);
            return Task.CompletedTask;
        }

        public virtual Task SignOutAsync(AuthenticationProperties properties)
        {
            Context.Session.Remove(Options.CookieName(Context));
            return Task.CompletedTask;
        }

        protected virtual string GetTlsTokenBinding()
        {
            var binding = Context.Features.Get<ITlsTokenBindingFeature>()?.GetProvidedTokenBindingId();
            return binding == null ? null : Convert.ToBase64String(binding);
        }
    }
}
