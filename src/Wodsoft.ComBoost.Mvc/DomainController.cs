using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Mvc
{
    internal class DomainController
    {
        internal static Type GetDomainServiceType(Type controllerType)
        {
            while (controllerType.BaseType != typeof(object))
            {
                controllerType = controllerType.BaseType;
            }
            if (!controllerType.IsGenericType)
                return null;
            if (controllerType.GetGenericTypeDefinition() != typeof(DomainController<>))
                return null;
            return controllerType.GetGenericArguments()[0];
        }
    }

    public abstract class DomainController<TDomainService> : IDomainController
        where TDomainService : class, IDomainService
    {
        [ControllerContext]
        public ControllerContext ControllerContext { get; set; }

        protected virtual TDomainService GetDomainService()
        {
            if (ControllerContext == null)
                throw new InvalidOperationException("ControllerContext is null.");
            return ControllerContext.HttpContext.RequestServices.GetRequiredService<TDomainService>();
        }

        protected virtual IDomainContext GetDomainContext()
        {
            if (ControllerContext == null)
                throw new InvalidOperationException("ControllerContext is null.");
            return ControllerContext.HttpContext.RequestServices.GetRequiredService<IDomainContextProvider>().GetContext();
        }
    }
}
