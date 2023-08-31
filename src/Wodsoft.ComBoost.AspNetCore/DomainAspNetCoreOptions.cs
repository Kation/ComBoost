using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Wodsoft.ComBoost.AspNetCore
{
    public class DomainAspNetCoreOptions
    {
        private Func<HttpContext, ClaimsPrincipal> _authenticationHandler = context => context.User ;
        public Func<HttpContext, ClaimsPrincipal> AuthenticationHandler
        {
            get => _authenticationHandler; set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                _authenticationHandler = value;
            }
        }
    }
}
