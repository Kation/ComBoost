using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    public interface IDomainServiceAccessor
    {
        IDomainService DomainService { get; set; }
    }
}
