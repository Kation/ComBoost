using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.AspNetCore
{
    /// <summary>
    /// Http领域上下文。
    /// </summary>
    public class HttpDomainContext : DomainContext
    {
        public HttpDomainContext(HttpContext httpContext) : base(httpContext.RequestServices, httpContext.RequestAborted)
        {
            HttpContext = httpContext;
        }

        /// <summary>
        /// 获取Http上下文。
        /// </summary>
        public HttpContext HttpContext { get; private set; }

        /// <summary>
        /// 获取值提供器。
        /// </summary>
        public HttpValueProvider ValueProvider { get { return GetValueProvider(); } }

        private HttpValueProvider _ValueProvider;
        /// <summary>
        /// 获取值提供器。
        /// </summary>
        /// <returns>返回Http值提供器。</returns>
        protected virtual HttpValueProvider GetValueProvider()
        {
            if (_ValueProvider == null)
                _ValueProvider = new HttpValueProvider(HttpContext);
            return _ValueProvider;
        }

        public override object GetService(Type serviceType)
        {
            if (serviceType == typeof(IValueProvider) || serviceType == typeof(IConfigurableValueProvider))
                return ValueProvider;
            return base.GetService(serviceType);
        }
    }
}
