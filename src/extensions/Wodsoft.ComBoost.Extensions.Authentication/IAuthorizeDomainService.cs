using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Extensions.Authentication
{
    public interface IAuthorizeDomainService
    {
        ValueTask<AuthorizeResult> Authorize(string username, string password);

        ValueTask<AuthorizeResult> Authorize(string token);        
    }
}
