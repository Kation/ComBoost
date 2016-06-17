using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Security
{
    public interface ISecurityProvider
    {
        IPermission GetPermission(IDictionary<string, string> properties);

        IPermission GetPermission(string name);
    }
}
