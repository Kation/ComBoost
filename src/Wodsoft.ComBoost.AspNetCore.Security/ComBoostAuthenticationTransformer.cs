using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;

namespace Wodsoft.ComBoost.Security
{
    public class ComBoostAuthenticationTransformer : IClaimsTransformation
    {
        public ComBoostAuthenticationTransformer(ISecurityProvider securityProvider)
        {
            SecurityProvider = securityProvider ?? throw new ArgumentNullException(nameof(securityProvider));
        }

        public ISecurityProvider SecurityProvider { get; private set; }

        public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            ClaimsPrincipal newPrincipal = new ComBoostPrincipal(SecurityProvider);
            newPrincipal.AddIdentities(principal.Identities);
            return Task.FromResult(newPrincipal);
        }
    }
}
