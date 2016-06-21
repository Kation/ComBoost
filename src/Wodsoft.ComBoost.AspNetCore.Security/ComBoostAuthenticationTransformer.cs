using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;

namespace Wodsoft.ComBoost.Security
{
    public class ComBoostAuthenticationTransformer : IClaimsTransformer
    {
        public Task<ClaimsPrincipal> TransformAsync(ClaimsTransformationContext context)
        {
            var securityProvider = context.Context.RequestServices.GetRequiredService<ISecurityProvider>();
            ClaimsPrincipal principal = new ComBoostPrincipal(securityProvider);
            principal.AddIdentities(context.Principal.Identities);
            return Task.FromResult(principal);
        }
    }
}
