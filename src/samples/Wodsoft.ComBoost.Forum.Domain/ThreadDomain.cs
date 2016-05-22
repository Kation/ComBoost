using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data;

namespace Wodsoft.ComBoost.Forum.Domain
{
    public class ThreadDomain : DomainService
    {
        public async Task Create([FromService] IAuthenticationProvider authentication, string title, string content)
        {
            //IAuthenticationProvider
            //EntityDomain<>
        }
    }
}
