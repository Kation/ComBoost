using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.AspNetCore
{
    public class HttpDomainContext : DomainContext
    {
        public HttpDomainContext(HttpContext httpContext) : base(httpContext.RequestServices, httpContext.RequestAborted)
        {
            HttpContext = httpContext;
        }

        public HttpContext HttpContext { get; private set; }

        public HttpValueProvider ValueProvider { get { return GetValueProvider(); } }

        private HttpValueProvider _ValueProvider;
        protected virtual HttpValueProvider GetValueProvider()
        {
            if (_ValueProvider == null)
                _ValueProvider = new HttpValueProvider(HttpContext);
            return _ValueProvider;
        }

        public override object GetService(Type serviceType)
        {
            if (serviceType == typeof(IValueProvider))
                return ValueProvider;
            return base.GetService(serviceType);
        }
    }
}
