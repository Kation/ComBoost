using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
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

        public Task<ClaimsPrincipal> GetUserAsync()
        {
            return Task.FromResult(User);
        }
    }
}
