using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;
using Wodsoft.ComBoost.AspNetCore;

namespace Wodsoft.ComBoost.Mvc
{
    public class MvcDomainContextProvider : HttpDomainContextProvider
    {
        public MvcDomainContextProvider(IActionContextAccessor actionContextAccessor) : base(actionContextAccessor.ActionContext.HttpContext)
        {
            if (actionContextAccessor == null)
                throw new ArgumentNullException(nameof(actionContextAccessor));
            ActionContext = actionContextAccessor.ActionContext;
        }

        protected MvcDomainContextProvider(ActionContext actionContext) : base(actionContext.HttpContext)
        {
            ActionContext = actionContext ?? throw new ArgumentNullException(nameof(actionContext));
        }

        protected ActionContext ActionContext { get; }

        public override IDomainContext GetContext()
        {
            return new MvcDomainContext(ActionContext);
        }
    }
}
