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
        private MockPrincipal _CurrentPrincipal;
        private ISecurityProvider _SecurityProvider;

        public MockAuthenticationProvider(ISecurityProvider securityProvider)
        {
            _SecurityProvider = securityProvider;
            _CurrentPrincipal = new MockPrincipal(securityProvider);
        }

        public IAuthentication GetAuthentication()
        {
            return _CurrentPrincipal;
        }

        public Task SignInAsync(IPermission permission)
        {
            if (permission == null)
                throw new ArgumentNullException(nameof(permission));
            _CurrentPrincipal = new MockPrincipal(permission, _SecurityProvider);
            return Task.CompletedTask;
        }

        public async Task<bool> SignInAsync(IDictionary<string, string> properties)
        {
            var permission = await _SecurityProvider.GetPermissionAsync(properties);
            if (permission == null)
                return false;
            _CurrentPrincipal = new MockPrincipal(permission, _SecurityProvider);
            return true;
        }

        public Task SignOutAsync()
        {
            _CurrentPrincipal = new MockPrincipal(_SecurityProvider);
            return Task.CompletedTask;
        }
    }
}
