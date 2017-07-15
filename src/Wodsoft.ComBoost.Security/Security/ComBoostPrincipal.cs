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

        public T GetUser<T>() where T : class
        {
            if (!Identity.IsAuthenticated)
                return null;
            string id = FindFirst(t => t.Type == ClaimTypes.NameIdentifier).Value;
            return (T)SecurityProvider.GetPermissionAsync(id).Result;
        }

        public Task<T> GetUserAsync<T>() where T : class
        {
            if (!Identity.IsAuthenticated)
                return Task.FromResult<T>(null);
            string id = FindFirst(t => t.Type == ClaimTypes.NameIdentifier).Value;
            return SecurityProvider.GetPermissionAsync(id).ContinueWith(t => (T)t.Result);
        }

        public bool IsInStaticRole(object role)
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role));
            if (!Identity.IsAuthenticated)
                return false;
            var roles = FindAll(t => t.Type == ClaimTypes.Role);
            if (roles.Any(t => t.Value == SecurityProvider.ConvertRoleToString(role)))
                return true;
            return false;
        }

        public bool IsInDynamicRole(object role)
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role));
            if (!Identity.IsAuthenticated)
                return false;
            string id = FindFirst(t => t.Type == ClaimTypes.NameIdentifier).Value;
            return SecurityProvider.GetPermissionAsync(id).Result.IsInRole(role);
        }

        public bool IsInRole(object role)
        {
            if (!Identity.IsAuthenticated)
                return false;
            if (IsInStaticRole(role))
                return true;
            return IsInDynamicRole(role);
        }

        public string GetUserId()
        {
            return FindFirst(t => t.Type == ClaimTypes.NameIdentifier)?.Value;
        }

        public string GetUserName()
        {
            return FindFirst(t => t.Type == ClaimTypes.Name)?.Value;
        }
    }
}
