using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Security;

namespace Wodsoft.ComBoost.Mock
{
    public class MockAuthenticationProvider : IAuthenticationProvider
    {
        private IPermission _CurrentPermission;
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
            _CurrentPermission = permission;
            return Task.CompletedTask;
        }

        public async Task<bool> SignInAsync(IDictionary<string, string> properties)
        {
            var permission = await _SecurityProvider.GetPermissionAsync(properties);
            if (permission == null)
                return false;
            _CurrentPermission = permission;
            return true;
        }

        public Task SignOutAsync()
        {
            _CurrentPermission = null;
            return Task.CompletedTask;
        }
    }
}
