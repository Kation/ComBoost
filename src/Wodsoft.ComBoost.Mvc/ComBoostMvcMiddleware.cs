using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Mvc
{
    /// <summary>
    /// ComBoost中间件。
    /// </summary>
    public class ComBoostMvcMiddleware
    {
        private readonly RequestDelegate _Next;
        private readonly IRouter _Router;

        /// <summary>
        /// 实例化ComBoost中间件。
        /// </summary>
        /// <param name="next">下一请求委托。</param>
        /// <param name="router">路由。</param>
        public ComBoostMvcMiddleware(RequestDelegate next, IRouter router)
        {
            _Next = next;
            _Router = router;
        }

        /// <summary>
        /// 执行。
        /// </summary>
        /// <param name="httpContext">Http上下文。</param>
        /// <returns></returns>
        public async Task Invoke(HttpContext httpContext)
        {
            var context = new RouteContext(httpContext);
            context.RouteData.Routers.Add(_Router);

            await _Router.RouteAsync(context);

            if (context.Handler != null)
            {
                httpContext.Features[typeof(IRoutingFeature)] = new RoutingFeature()
                {
                    RouteData = context.RouteData,
                };
            }

            // proceed to next...
            await _Next(httpContext);
        }
    }
}
