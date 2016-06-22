using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Security;

namespace Wodsoft.ComBoost.Security
{
    public interface IAuthenticationProvider
    {
        IAuthentication GetAuthentication();

        Task<bool> SignInAsync(IDictionary<string, string> properties);

        Task SignInAsync(IPermission permission);

        Task SignOutAsync();
    }
}
