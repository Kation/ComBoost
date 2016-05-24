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

namespace Wodsoft.ComBoost.Security
{
    public class ComBoostAuthenticationMiddleware : AuthenticationMiddleware<ComBoostAuthenticationOptions>
    {
        public ComBoostAuthenticationMiddleware(RequestDelegate next, IOptions<ComBoostAuthenticationOptions> options, ILoggerFactory loggerFactory, UrlEncoder encoder)
            : base(next, options, loggerFactory, encoder)
        {
        }

        protected override AuthenticationHandler<ComBoostAuthenticationOptions> CreateHandler()
        {
            return new ComBoostAuthenticationHandler();
        }
    }
}
