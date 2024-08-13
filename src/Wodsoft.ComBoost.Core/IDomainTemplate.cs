using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost
{
    public interface IDomainTemplate
    {
        IDomainContext Context { get; set; }
    }
}
