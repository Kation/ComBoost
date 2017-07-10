using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Security
{
    public interface IAuthentication : System.Security.Principal.IPrincipal
    {
        bool IsInRole(object role);

        T GetUser<T>();

        string GetUserId();

        string GetUserName();
    }
}
