using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Security
{
    public class ComBoostAnonymousPrincipal : IAuthentication
    {
        public IIdentity Identity => new ComBoostAnonymousIdentity();

        public T GetUser<T>() where T : class
        {
            return null;
        }

        public Task<T> GetUserAsync<T>() where T : class
        {
            return Task.FromResult<T>(null);
        }

        public string GetUserId()
        {
            return null;
        }

        public string GetUserName()
        {
            return null;
        }

        public bool IsInDynamicRole(object role)
        {
            return false;
        }

        public bool IsInRole(object role)
        {
            return false;
        }

        public bool IsInRole(string role)
        {
            return false;
        }

        public bool IsInStaticRole(object role)
        {
            return false;
        }
    }
}
