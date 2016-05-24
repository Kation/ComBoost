using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Security
{
    public static class ComBoostPrincipalExtensions
    {
        public static TPermission GetPermission<TPermission>(this IPrincipal principal)
            where TPermission : class, IPermission
        {
            return null;
        }
    }
}
