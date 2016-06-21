using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Security
{
    public interface ISecurityProvider
    {
        Task<IPermission> GetPermissionAsync(IDictionary<string, string> properties);

        Task<IPermission> GetPermissionAsync(string identity);

        string ConvertRoleToString(object role);
    }
}
