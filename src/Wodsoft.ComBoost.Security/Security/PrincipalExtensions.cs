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
    public static class PrincipalExtensions
    {
        [Obsolete("请使用GetUserAsync方法替代GetPermission。")]
        public static Task<TPermission> GetPermission<TPermission>(this IPrincipal principal)
            where TPermission : class, IPermission
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));
            if (!principal.Identity.IsAuthenticated)
                return Task.FromResult<TPermission>(null);
            IAuthentication authentication = principal as IAuthentication;
            if (authentication == null)
                return Task.FromResult<TPermission>(null);
            return authentication.GetUserAsync<TPermission>();
        }

        public static T GetUser<T>(this IPrincipal principal)
             where T : class
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));
            if (!principal.Identity.IsAuthenticated)
                return null;
            IAuthentication authentication = principal as IAuthentication;
            if (authentication == null)
                return null;
            return authentication.GetUser<T>();
        }

        public static Task<T> GetUserAsync<T>(this IPrincipal principal)
             where T : class
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));
            if (!principal.Identity.IsAuthenticated)
                return Task.FromResult<T>(null);
            IAuthentication authentication = principal as IAuthentication;
            if (authentication == null)
                return Task.FromResult<T>(null);
            return authentication.GetUserAsync<T>();
        }

        public static bool IsInStaticRole(this IPrincipal principal, object role)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));
            if (role == null)
                throw new ArgumentNullException(nameof(role));
            IAuthentication authentication = principal as IAuthentication;
            if (authentication == null)
                return false;
            return authentication.IsInStaticRole(role);
        }

        public static bool IsInDynamicRole(this IPrincipal principal, object role)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));
            if (role == null)
                throw new ArgumentNullException(nameof(role));
            IAuthentication authentication = principal as IAuthentication;
            if (authentication == null)
                return false;
            return authentication.IsInDynamicRole(role);
        }

        public static bool IsInRole(this IPrincipal principal, object role)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));
            if (role == null)
                throw new ArgumentNullException(nameof(role));
            IAuthentication authentication = principal as IAuthentication;
            if (authentication == null)
                return false;
            return authentication.IsInRole(role);

        }
    }
}
