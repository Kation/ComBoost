using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    public interface IPermission
    {
        bool IsInRole(object role);

        object[] GetStaticRoles();

        string Name { get; }

        string Identity { get; }
    }
}
