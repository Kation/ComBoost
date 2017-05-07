using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Mvc
{
    public abstract class DomainViewComponent : ViewComponent
    {
        public DomainViewComponent()
        {
            DomainProvider = HttpContext.RequestServices.GetRequiredService<IDomainServiceProvider>();
        }

        protected IDomainServiceProvider DomainProvider { get; private set; }

        protected virtual ViewComponentDomainContext CreateDomainContext()
        {
            return new ViewComponentDomainContext(this);
        }
    }
}
