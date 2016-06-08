using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data;
using Wodsoft.ComBoost.Security;

namespace Wodsoft.ComBoost.Forum.Domain
{
    public class ThreadDomain : DomainService
    {
        public async Task Create([FromService] IAuthenticationProvider authentication, [FromValue] string title, [FromValue] string content)
        {
            //IAuthenticationProvider
            //EntityDomain<>
        }
    }
}
