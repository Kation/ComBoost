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
            //AutomaticChallenge = true;
        }

        //public Func<HttpContext, string> CookiePath
    }

    
}
