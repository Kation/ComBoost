using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Forum.Domain
{
    public static class DomainServiceExtensions
    {
        public static void UseForum(this IDomainProvider provider)
        {
            provider.RegisterService<ThreadDomain>();
        }
    }
}
