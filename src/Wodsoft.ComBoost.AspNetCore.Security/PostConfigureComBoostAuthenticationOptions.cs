using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost.Security
{
    public class PostConfigureComBoostAuthenticationOptions : IPostConfigureOptions<ComBoostAuthenticationOptions>
    {
        private IDataProtectionProvider _DataProtectionProvider;

        public PostConfigureComBoostAuthenticationOptions(IDataProtectionProvider dataProtectionProvider)
        {
            _DataProtectionProvider = dataProtectionProvider;
        }

        public void PostConfigure(string name, ComBoostAuthenticationOptions options)
        {
            if (options.TicketDataFormat == null)
            {
                var dataProtector = _DataProtectionProvider.CreateProtector("ComBoost", name, "v2");
                options.TicketDataFormat = new TicketDataFormat(dataProtector);
            }
        }
    }
}
