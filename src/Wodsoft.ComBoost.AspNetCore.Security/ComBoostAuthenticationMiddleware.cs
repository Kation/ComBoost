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

namespace Wodsoft.ComBoost.Security
{
    public class ComBoostAuthenticationMiddleware : AuthenticationMiddleware<ComBoostAuthenticationOptions>
    {
        public ComBoostAuthenticationMiddleware(RequestDelegate next, IOptions<ComBoostAuthenticationOptions> options, ILoggerFactory loggerFactory,
            IDataProtectionProvider dataProtectionProvider, UrlEncoder encoder)
            : base(next, options, loggerFactory, encoder)
        {            
            if (Options.TicketDataFormat == null)
            {
                var provider = Options.DataProtectionProvider ?? dataProtectionProvider;
                var dataProtector = provider.CreateProtector(typeof(ComBoostAuthenticationMiddleware).FullName, Options.AuthenticationScheme);
                Options.TicketDataFormat = new TicketDataFormat(dataProtector);
            }
        }

        protected override AuthenticationHandler<ComBoostAuthenticationOptions> CreateHandler()
        {
            return new ComBoostAuthenticationHandler();
        }
    }
}
