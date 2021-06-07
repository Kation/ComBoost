using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Wodsoft.ComBoost.AspNetCore;

namespace Wodsoft.ComBoost.Mvc
{
    public class MvcDomainContext : HttpDomainContext
    {
        public MvcDomainContext(ActionContext actionContext)
            : base(actionContext.HttpContext)
        {
            ActionContext = actionContext;
        }

        public ActionContext ActionContext { get; private set; }

        private MvcValueProvider _ValueProvider;
        protected override HttpValueProvider GetValueProvider()
        {
            if (_ValueProvider == null)
                _ValueProvider = new MvcValueProvider(ActionContext);
            return _ValueProvider;
        }
    }
}
