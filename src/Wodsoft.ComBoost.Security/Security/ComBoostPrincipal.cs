using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Security
{
    public class ComBoostPrincipal : ClaimsPrincipal, IAuthentication
    {
        public ComBoostPrincipal(ISecurityProvider securityProvider)
        {
            if (securityProvider == null)
                throw new ArgumentNullException(nameof(securityProvider));
            SecurityProvider = securityProvider;
        }

        public ISecurityProvider SecurityProvider { get; private set; }

        public T GetUser<T>()
        {
            if (!Identity.IsAuthenticated)
                return default(T);
            string id = ((ClaimsIdentity)Identity).FindFirst(t => t.Type == ClaimTypes.NameIdentifier).Value;
            return (T)SecurityProvider.GetPermissionAsync(id).Result;
        }

        public bool IsInRole(object role)
        {
            if (!Identity.IsAuthenticated)
                return false;
            string id = ((ClaimsIdentity)Identity).FindFirst(t => t.Type == ClaimTypes.NameIdentifier).Value;
            return SecurityProvider.GetPermissionAsync(id).Result.IsInRole(role);
        }
    }
}
