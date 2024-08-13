using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Wodsoft.ComBoost.AspNetCore;
using System.Security.Claims;

namespace Wodsoft.ComBoost.Mvc
{
    public class MvcDomainContext : HttpDomainContext
    {
        public MvcDomainContext(ActionContext actionContext, ClaimsPrincipal user)
            : base(actionContext.HttpContext, user)
        {
            ActionContext = actionContext;
        }

        public ActionContext ActionContext { get; private set; }

        private MvcValueProvider? _valueProvider;
        protected override HttpValueProvider GetValueProvider()
        {
            if (_valueProvider == null)
                _valueProvider = new MvcValueProvider(ActionContext);
            return _valueProvider;
        }
    }
}
