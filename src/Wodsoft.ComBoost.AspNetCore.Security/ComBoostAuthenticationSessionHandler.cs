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
    public class ComBoostAuthenticationSessionHandler : AuthenticationHandler<ComBoostAuthenticationOptions>
    {
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var ticketData = Context.Session.GetString(Options.CookieName(Context));
            if (ticketData == null)
                return Task.FromResult(AuthenticateResult.Skip());
            try
            {
                var ticket = Options.TicketDataFormat.Unprotect(ticketData, GetTlsTokenBinding());
                if (ticket.Properties.ExpiresUtc < DateTimeOffset.Now)
                    return Task.FromResult(AuthenticateResult.Skip());
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

        protected override Task HandleSignInAsync(SignInContext context)
        {
            var expireTime = Options.ExpireTime(Context);
            var expireDate = expireTime.HasValue ? (DateTimeOffset?)DateTimeOffset.Now.Add(expireTime.Value) : null;
            var ticket = new AuthenticationTicket(context.Principal, new AuthenticationProperties() { ExpiresUtc = expireDate }, Options.AuthenticationScheme);
            var ticketValue = Options.TicketDataFormat.Protect(ticket, GetTlsTokenBinding());
            Context.Session.SetString(Options.CookieName(Context), ticketValue);
            return Task.CompletedTask;
        }

        protected override Task HandleSignOutAsync(SignOutContext context)
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
