using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.AspNetCore
{
    /// <summary>
    /// Http领域上下文。
    /// </summary>
    public class HttpDomainContext : DomainContext
    {
        public HttpDomainContext(HttpContext httpContext, ClaimsPrincipal user) : base(httpContext.RequestServices, httpContext.RequestAborted)
        {
            HttpContext = httpContext;
            User = user;
        }

        /// <summary>
        /// 获取Http上下文。
        /// </summary>
        public HttpContext HttpContext { get; private set; }

        /// <summary>
        /// 获取值提供器。
        /// </summary>
        public override IValueProvider ValueProvider { get { return GetValueProvider(); } }

        public override ClaimsPrincipal User { get; }

        private HttpValueProvider? _valueProvider;
        /// <summary>
        /// 获取值提供器。
        /// </summary>
        /// <returns>返回Http值提供器。</returns>
        protected virtual HttpValueProvider GetValueProvider()
        {
            if (_valueProvider == null)
                _valueProvider = new HttpValueProvider(HttpContext);
            return _valueProvider;
        }
    }
}
