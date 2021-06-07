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

        protected virtual ControllerDomainContext CreateDomainContext()
        {
            return new ControllerDomainContext(this);
        }

        protected virtual EmptyDomainContext CreateEmptyDomainContext()
        {
            return new EmptyDomainContext(HttpContext.RequestServices, HttpContext.RequestAborted);
        }

        protected T GetDomainTemplate<T>() where T : IDomainTemplate
        {
            var template = HttpContext.RequestServices.GetRequiredService<T>();
            template.Context = CreateDomainContext();
            return template;
        }
    }
}
