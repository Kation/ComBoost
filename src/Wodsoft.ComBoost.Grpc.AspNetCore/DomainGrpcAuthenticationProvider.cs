using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using Wodsoft.ComBoost.Security;

namespace Wodsoft.ComBoost.Grpc.AspNetCore
{
    public class DomainGrpcAuthenticationProvider : IAuthenticationProvider
    {
        public DomainGrpcAuthenticationProvider(ClaimsPrincipal principal)
        {
            User = principal ?? throw new ArgumentNullException(nameof(principal));
        }

        public ClaimsPrincipal User { get; }

        public bool IsInRole(string role)
        {
            return User.IsInRole(role);
        }
    }
}
