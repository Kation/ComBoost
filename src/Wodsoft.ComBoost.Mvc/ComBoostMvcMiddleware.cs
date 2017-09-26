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
    public class ComBoostMvcMiddleware
    {
        private readonly RequestDelegate _Next;
        private readonly IRouter _Router;

        public ComBoostMvcMiddleware(RequestDelegate next, IRouter router)
        {
            _Next = next;
            _Router = router;
        }

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
