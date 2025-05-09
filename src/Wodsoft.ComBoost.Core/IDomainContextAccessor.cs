using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost
{
    public interface IDomainContextAccessor
    {
        IDomainContext Context { get; set; }
    }
}
