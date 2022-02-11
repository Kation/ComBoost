using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost.AspNetCore
{
    public class HttpDomainContextProvider : IDomainContextProvider
    {
        public HttpDomainContextProvider(IHttpContextAccessor httpContextAccessor)
        {
            HttpContext = httpContextAccessor?.HttpContext;
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
            return new HttpDomainContext(HttpContext);
        }
    }
}
