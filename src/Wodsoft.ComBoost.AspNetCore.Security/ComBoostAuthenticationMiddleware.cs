using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;

namespace Wodsoft.ComBoost.Security
{
    public class ComBoostAuthenticationMiddleware : AuthenticationMiddleware<ComBoostAuthenticationOptions>
    {
        private IServiceProvider _ServiceProvider;

        public ComBoostAuthenticationMiddleware(RequestDelegate next, IOptions<ComBoostAuthenticationOptions> options, ILoggerFactory loggerFactory,
            IDataProtectionProvider dataProtectionProvider, UrlEncoder encoder, IServiceProvider serviceProvider)
            : base(next, options, loggerFactory, encoder)
        {
            if (Options.TicketDataFormat == null)
            {
                var provider = Options.DataProtectionProvider ?? dataProtectionProvider;
                var dataProtector = provider.CreateProtector(typeof(ComBoostAuthenticationMiddleware).FullName, Options.AuthenticationScheme);
                Options.TicketDataFormat = new TicketDataFormat(dataProtector);
            }
            _ServiceProvider = serviceProvider;
        }

        protected override AuthenticationHandler<ComBoostAuthenticationOptions> CreateHandler()
        {
            var handler = _ServiceProvider.GetService<AuthenticationHandler<ComBoostAuthenticationOptions>>();
            if (handler == null)
                handler = new ComBoostAuthenticationCookieHandler();
            return handler;
        }
    }
}
