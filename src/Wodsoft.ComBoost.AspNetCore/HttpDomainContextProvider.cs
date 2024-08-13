using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost.AspNetCore
{
    public class HttpDomainContextProvider : IDomainContextProvider
    {
        private readonly DomainAspNetCoreOptions _options;
        public HttpDomainContextProvider(IHttpContextAccessor httpContextAccessor, IOptions<DomainAspNetCoreOptions> options)
        {
            HttpContext = httpContextAccessor?.HttpContext;
            _options = options.Value;
        }

        protected HttpDomainContextProvider(HttpContext? httpContext)
        {
            HttpContext = httpContext;
        }

        public virtual bool CanProvide => HttpContext != null;

        protected HttpContext? HttpContext { get; }

        public virtual IDomainContext GetContext()
        {
            if (HttpContext == null)
                throw new NotSupportedException("There is no http context currently.");            
            return new HttpDomainContext(HttpContext, _options.AuthenticationHandler(HttpContext));
        }
    }
}
