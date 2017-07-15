using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Security;

namespace Wodsoft.ComBoost.Mock
{
    public class MockPrincipal : IAuthentication
    {
        public MockPrincipal(ISecurityProvider securityProvider) : this(null, securityProvider)
        {

        }

        public MockPrincipal(IPermission permission, ISecurityProvider securityProvider)
        {
            if (securityProvider == null)
                throw new ArgumentNullException(nameof(securityProvider));
            SecurityProvider = securityProvider;
            Identity = new MockIdentity(permission?.Name);
            UserId = permission?.Identity;
        }

        public ISecurityProvider SecurityProvider { get; private set; }
        private string UserId;

        public MockIdentity Identity { get; private set; }
        IIdentity IPrincipal.Identity { get { return Identity; } }

        public T GetUser<T>() where T : class
        {
            if (UserId == null)
                return null;
            return SecurityProvider.GetPermissionAsync(UserId).Result as T;
        }

        public Task<T> GetUserAsync<T>() where T : class
        {
            return SecurityProvider.GetPermissionAsync(UserId).ContinueWith(t => t.Result as T);
        }

        public bool IsInRole(string role)
        {
            return IsInRole((object)role);
        }

        public bool IsInRole(object role)
        {
            if (UserId != null)
            {
                var permission = SecurityProvider.GetPermissionAsync(UserId).Result;
                return permission.IsInRole(role);
            }
            return false;
        }

        public string GetUserId()
        {
            return UserId;
        }

        public string GetUserName()
        {
            return Identity?.Name;
        }
    }
}
