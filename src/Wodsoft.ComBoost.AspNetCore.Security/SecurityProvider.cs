using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Security
{
    public class SecurityProvider : ISecurityProvider
    {
        public IPermission GetRoleObject(IIdentity identity)
        {
            //Microsoft.AspNetCore.Http
            return null;
        }
    }
}
