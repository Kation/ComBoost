using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;

namespace Wodsoft.ComBoost.Mvc
{
    public abstract class MvcDomainContext : DomainContext
    {
        public MvcDomainContext(ActionContext actionContext)
            : base(actionContext.HttpContext.RequestServices, actionContext.HttpContext.RequestAborted)
        {
            if (actionContext == null)
                throw new ArgumentNullException(nameof(actionContext));
            ActionContext = actionContext;
        }

        public ActionContext ActionContext { get; private set; }

        protected abstract MvcValueProvider GetValueProvider();

        public MvcValueProvider ValueProvider { get { return GetValueProvider(); } }

        public override object GetService(Type serviceType)
        {
            if (serviceType == typeof(IValueProvider))
                return ValueProvider;
            return base.GetService(serviceType);
        }
    }
}
