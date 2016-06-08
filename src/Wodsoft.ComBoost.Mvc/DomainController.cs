using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Mvc
{
    public class DomainController : Controller
    {
        protected IDomainProvider DomainProvider { get; private set; }

        public DomainController()
        {
            DomainProvider = HttpContext.RequestServices.GetRequiredService<IDomainProvider>();
        }
        
        protected virtual IDomainContext CreateDomainContext()
        {
            return new MvcDomainContext(this);
        }
    }
}
