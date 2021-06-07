using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Wodsoft.ComBoost.Extensions.Authentication
{
    public abstract class AuthorizeDomainService : DomainService
    {
        protected AuthorizeResult NotMatched()
        {
            return new AuthorizeResult();
        }

        protected AuthorizeResult Locked()
        {
            return new AuthorizeResult
            {
                IsMatched = true,
                IsLocked = true
            };
        }

        protected AuthorizeResult Disabled()
        {
            return new AuthorizeResult
            {
                IsMatched = true,
                IsEnabled = false
            };
        }

        protected AuthorizeResult NotValid()
        {
            return new AuthorizeResult
            {
                IsMatched = true,
                IsEnabled = true
            };
        }

        protected AuthorizeResult Valid(ClaimsPrincipal principal)
        {
            return new AuthorizeResult
            {
                IsMatched = true,
                IsValidated = true,
                IsEnabled = true,
                User = principal
            };
        }
    }
}
