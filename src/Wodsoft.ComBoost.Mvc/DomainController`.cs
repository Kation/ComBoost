using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

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
