using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Security;

namespace Wodsoft.ComBoost.Mock
{
    public class AnonymousAuthenticationProvider : IAuthenticationProvider
    {
        public AnonymousAuthenticationProvider()
        {
            ClaimsIdentity identity = new ClaimsIdentity("Anonymous");
            User = new ClaimsPrincipal(identity);
        }

        public ClaimsPrincipal User { get; }

        public bool IsInRole(string role)
        {
            return false;
        }
    }
}
