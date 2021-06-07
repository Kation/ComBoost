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
            if (httpContextAccessor == null)
                throw new ArgumentNullException(nameof(httpContextAccessor));
            HttpContext = httpContextAccessor.HttpContext;
        }

        protected HttpDomainContextProvider(HttpContext httpContext)
        {
            HttpContext = httpContext ?? throw new ArgumentNullException(nameof(httpContext));
        }

        protected HttpContext HttpContext { get; }

        public virtual IDomainContext GetContext()
        {
            return new HttpDomainContext(HttpContext);
        }
    }
}
