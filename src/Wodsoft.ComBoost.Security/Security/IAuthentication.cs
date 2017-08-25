using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Security
{
    public interface IAuthentication : System.Security.Principal.IPrincipal
    {
        bool IsInStaticRole(object role);

        bool IsInDynamicRole(object role);
        
        T GetUser<T>() where T : class;

        Task<T> GetUserAsync<T>() where T : class;

        string GetUserId();

        string GetUserName();
    }
}
