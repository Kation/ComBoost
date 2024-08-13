using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost.Mvc
{
    public interface IDomainAction<T>
        where T : class, IDomainTemplate
    {
        
    }
}
