using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Wodsoft.ComBoost;
using Wodsoft.ComBoost.Security;

namespace Microsoft.AspNetCore.Builder
{
    public static class ComBoostPrincipalExtensions
    {
        public static async Task<TPermission> GetPermission<TPermission>(this IPrincipal principal)
            where TPermission : class, IPermission
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));
            if (!principal.Identity.IsAuthenticated)
                return null;
            ComBoostPrincipal comboostPrincipal = principal as ComBoostPrincipal;
            if (comboostPrincipal == null)
                return null;
            string identity = comboostPrincipal.Claims.First(t => t.Type == ClaimTypes.NameIdentifier).Value;
            var permission = await comboostPrincipal.SecurityProvider.GetPermissionAsync(identity);
            return permission as TPermission;
        }

        public static bool IsInStaticRole(this IPrincipal principal, object role)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));
            if (role == null)
                throw new ArgumentNullException(nameof(role));
            if (!principal.Identity.IsAuthenticated)
                return false;
            ComBoostPrincipal comboostPrincipal = principal as ComBoostPrincipal;
            if (comboostPrincipal == null)
                return false;
            var roleName = comboostPrincipal.SecurityProvider.ConvertRoleToString(role);
            return comboostPrincipal.IsInRole(roleName);
        }

        public static bool IsInDynamicRole(this IPrincipal principal, object role)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));
            if (role == null)
                throw new ArgumentNullException(nameof(role));
            if (!principal.Identity.IsAuthenticated)
                return false;
            ComBoostPrincipal comboostPrincipal = principal as ComBoostPrincipal;
            if (comboostPrincipal == null)
                return false;
            return false;
        }
    }
}
