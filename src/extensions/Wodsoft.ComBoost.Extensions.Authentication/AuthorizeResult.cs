using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Wodsoft.ComBoost.Extensions.Authentication
{
    public class AuthorizeResult
    {
        public bool IsValidated { get; set; }

        public bool IsMatched { get; set; }

        public bool IsLocked { get; set; }

        public bool IsEnabled { get; set; }

        public string FailureMessage { get; set; }

        public ClaimsPrincipal User { get; set; }
    }
}
