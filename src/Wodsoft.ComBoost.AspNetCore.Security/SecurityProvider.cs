using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Security
{
    public class SecurityProvider : ISecurityProvider
    {
        public object ConvertRoleFromString(string role)
        {
            throw new NotImplementedException();
        }

        public string ConvertRoleToString(object role)
        {
            throw new NotImplementedException();
        }

        public Task<IPermission> GetPermissionAsync(string name)
        {
            throw new NotImplementedException();
        }

        public Task<IPermission> GetPermissionAsync(IDictionary<string, string> properties)
        {
            throw new NotImplementedException();
        }

        public IPermission GetRoleObject(IPrincipal principal)
        {
            throw new NotImplementedException();
        }

    }
}
