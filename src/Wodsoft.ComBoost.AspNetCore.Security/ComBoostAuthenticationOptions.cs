using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Builder
{
    public class ComBoostAuthenticationOptions : AuthenticationOptions
    {
        public ComBoostAuthenticationOptions()
        {
            AuthenticationScheme = "ComBoost";
            AutomaticAuthenticate = true;
            //TicketDataFormat = new TicketDataFormat()
            CookieName = c => "ComBoostAuthentication";
            ExpireTime = c => TimeSpan.FromMinutes(15);
            //LoginPath = c => "/Account/Login";
            //AutomaticChallenge = true;
        }
        public IDataProtectionProvider DataProtectionProvider { get; set; }

        public ISecureDataFormat<AuthenticationTicket> TicketDataFormat { get; set; }

        public Func<HttpContext, string> CookiePath { get; set; }

        public Func<HttpContext, string> CookieDomain { get; set; }

        public Func<HttpContext, string> CookieName { get; set; }

        public Func<HttpContext, TimeSpan> ExpireTime { get; set; }

        public Func<HttpContext, string> LoginPath { get; set; }

        public Func<HttpContext, string> LogoutPath { get; set; }
    }


}
