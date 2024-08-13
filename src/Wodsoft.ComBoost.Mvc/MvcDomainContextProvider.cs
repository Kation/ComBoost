using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using Wodsoft.ComBoost.AspNetCore;

namespace Wodsoft.ComBoost.Mvc
{
    public class MvcDomainContextProvider : HttpDomainContextProvider
    {
        private readonly DomainAspNetCoreOptions _options;
        public MvcDomainContextProvider(IActionContextAccessor actionContextAccessor, IOptions<DomainAspNetCoreOptions> options) : base(actionContextAccessor?.ActionContext?.HttpContext)
        {
            ActionContext = actionContextAccessor?.ActionContext;
            _options = options.Value;
        }

        protected MvcDomainContextProvider(ActionContext actionContext) : base(actionContext?.HttpContext)
        {
            ActionContext = actionContext;
        }

        protected ActionContext? ActionContext { get; }

        public override bool CanProvide => ActionContext != null;

        public override IDomainContext GetContext()
        {
            if (ActionContext == null)
                throw new NotSupportedException("There is no action context currently.");
            return new MvcDomainContext(ActionContext, _options.AuthenticationHandler(HttpContext));
        }
    }
}
