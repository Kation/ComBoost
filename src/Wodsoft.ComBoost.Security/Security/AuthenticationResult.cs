using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Wodsoft.ComBoost.Security
{
    public class AuthenticationResult
    {
        protected AuthenticationResult() { }

        public bool IsSuccess { get; protected set; }

        public ClaimsPrincipal Principal { get; protected set; }

        public static AuthenticationResult Fail()
        {
            return new AuthenticationResult
            {
                IsSuccess = false
            };
        }

        public static AuthenticationResult Success(ClaimsPrincipal principal)
        {
            return new AuthenticationResult
            {
                IsSuccess = true,
                Principal = principal
            };
        }
    }
}
