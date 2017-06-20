using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Security;

namespace Wodsoft.ComBoost.Mock
{
    public class MockAuthenticationProvider : IAuthenticationProvider
    {
        private ComBoostPrincipal _CurrentPrincipal;
        private ISecurityProvider _SecurityProvider;

        public MockAuthenticationProvider(ISecurityProvider securityProvider)
        {
            _SecurityProvider = securityProvider;
            _CurrentPrincipal = new ComBoostPrincipal(securityProvider);
        }

        public IAuthentication GetAuthentication()
        {
            return _CurrentPrincipal;
        }

        public Task SignInAsync(IPermission permission)
        {
            if (permission == null)
                throw new ArgumentNullException(nameof(permission));
            _CurrentPrincipal = new ComBoostPrincipal(_SecurityProvider);
            ClaimsIdentity identity = new ClaimsIdentity("ComBoostAuthentication", ClaimTypes.Name, ClaimTypes.Role);
            identity.AddClaims(permission.GetStaticRoles().Select(t => new Claim(ClaimTypes.Role, _SecurityProvider.ConvertRoleToString(t))));
            identity.AddClaim(new Claim(ClaimTypes.Name, permission.Name));
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, permission.Identity));
            _CurrentPrincipal.AddIdentity(identity);
            return Task.CompletedTask;
        }

        public async Task<bool> SignInAsync(IDictionary<string, string> properties)
        {
            var permission = await _SecurityProvider.GetPermissionAsync(properties);
            if (permission == null)
                return false;
            _CurrentPrincipal = new ComBoostPrincipal(_SecurityProvider);
            ClaimsIdentity identity = new ClaimsIdentity("ComBoostAuthentication", ClaimTypes.Name, ClaimTypes.Role);
            identity.AddClaims(permission.GetStaticRoles().Select(t => new Claim(ClaimTypes.Role, _SecurityProvider.ConvertRoleToString(t))));
            identity.AddClaim(new Claim(ClaimTypes.Name, permission.Name));
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, permission.Identity));
            _CurrentPrincipal.AddIdentity(identity);
            return true;
        }

        public Task SignOutAsync()
        {
            _CurrentPrincipal = new ComBoostPrincipal(_SecurityProvider);
            return Task.CompletedTask;
        }
    }
}
