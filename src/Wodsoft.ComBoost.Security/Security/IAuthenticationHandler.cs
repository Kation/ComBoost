using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Security
{
    public interface IAuthenticationHandler
    {
        Task<AuthenticationResult> AuthenticateAsync();
    }
}
