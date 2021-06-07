using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost
{
    public class ComBoostOptions
    {
        public IList<IDomainServiceFilter> GlobalFilters { get; }

        public void AddFilter<TDomainService, TFilter>()
            where TDomainService : class, IDomainService
            where TFilter : class, IDomainServiceFilter
        {

        }
    }
}
