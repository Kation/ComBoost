using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;

namespace Wodsoft.ComBoost.Security
{
    public class ComBoostAnonymousIdentity : IIdentity
    {
        public string AuthenticationType => "Anonymous";

        public bool IsAuthenticated => false;

        public string Name => null;
    }
}
