using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Filters;

namespace Wodsoft.ComBoost.Mvc
{
    public class DomainController<TService> : DomainController
        where TService : class, IDomainProvider, new()
    {
        public DomainController()
        {
            AddDomain(new TService());
        }
    }
}
