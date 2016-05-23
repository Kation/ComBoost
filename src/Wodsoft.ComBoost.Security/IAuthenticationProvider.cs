using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Security;

namespace Wodsoft.ComBoost
{
    public interface IAuthenticationProvider
    {
        IAuthentication GetAuthentication();
    }
}
