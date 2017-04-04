using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Wodsoft.ComBoost;
using Wodsoft.ComBoost.Security;

namespace Wodsoft.ComBoost.Security
{
    public static class ComBoostPrincipalExtensions
    {
        public static async Task<TPermission> GetPermission<TPermission>(this IPrincipal principal)
            where TPermission : IPermission
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));
            if (!principal.Identity.IsAuthenticated)
                return default(TPermission);
            ComBoostPrincipal comboostPrincipal = principal as ComBoostPrincipal;
            if (comboostPrincipal == null)
                return default(TPermission);
            string identity = comboostPrincipal.Claims.First(t => t.Type == ClaimTypes.NameIdentifier).Value;
            var permission = await comboostPrincipal.SecurityProvider.GetPermissionAsync(identity);
            return (TPermission)permission;
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
            var roles = comboostPrincipal.FindAll(t => t.Type == ClaimTypes.Role);
            if (roles.Any(t => t.Value == comboostPrincipal.SecurityProvider.ConvertRoleToString(role)))
                return true;
            return false;
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
            string id = comboostPrincipal.FindFirst(t => t.Type == ClaimTypes.NameIdentifier).Value;
            return comboostPrincipal.SecurityProvider.GetPermissionAsync(id).Result.IsInRole(role);
        }
    }
}
