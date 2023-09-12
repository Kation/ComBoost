using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Wodsoft.ComBoost;
using Wodsoft.ComBoost.AspNetCore;

namespace Wodsoft.ComBoost.AspNetCore
{
    //public class ComBoostMiddleware
    //{
    //    private RequestDelegate _Next;

    //    public ComBoostMiddleware(RequestDelegate next)
    //    {
    //        _Next = next;
    //    }

    //    public Task Invoke(HttpContext httpContext)
    //    {
    //        RequestScope.Current = new RequestScope();
    //        return _Next(httpContext);
    //    }
    //}
}

namespace Microsoft.AspNetCore.Builder
{
    public static class DomainAspNetCoreEndpointExtensions
    {
#if !NETCOREAPP2_1

        public static void MapHealthCheck(this IEndpointRouteBuilder endpoint, string path)
        {
            if (endpoint == null)
                throw new ArgumentNullException(nameof(endpoint));
            endpoint.Map(path, _HealthCheckDelegate);
        }

        public static void MapDomainEndpoint(this IEndpointRouteBuilder endpoint)
        {
            if (endpoint == null)
                throw new ArgumentNullException(nameof(endpoint));
            var types = endpoint.ServiceProvider.GetRequiredService<IOptions<DomainEndpointOptions>>().Value.Types;
            foreach (var type in types.Distinct())
            {
                var domainEndpoint = (DomainEndpoint)ActivatorUtilities.CreateInstance(endpoint.ServiceProvider, type);
                endpoint.Map(domainEndpoint.EndpointTemplate, domainEndpoint.HandleRequest);
            }
        }
#endif
        public static void UseDomainEndpoint(this IApplicationBuilder app)
        {
            if (app == null)
                throw new ArgumentNullException(nameof(app));
            var types = app.ApplicationServices.GetRequiredService<IOptions<DomainEndpointOptions>>().Value.Types;
            foreach (var type in types.Distinct())
            {
                var domainEndpoint = (DomainEndpoint)ActivatorUtilities.CreateInstance(app.ApplicationServices, type);
                var handler = new RouteHandler(domainEndpoint.HandleRequest);
                var routeBuilder = new RouteBuilder(app, handler);
                routeBuilder.MapRoute("Domain Template Endpoint", domainEndpoint.EndpointTemplate);
                var router = routeBuilder.Build();
                app.UseRouter(router);
            }
        }

        private static RequestDelegate _HealthCheckDelegate = httpContext =>
        {
            var providers = httpContext.RequestServices.GetServices<IHealthStateProvider>();
            if (providers.Any())
            {
                var state = providers.Max(t => t.State);
                if (state != HealthState.Healthy)
                    httpContext.Response.StatusCode = 503;
                else
                    httpContext.Response.StatusCode = 203;
            }
            else
            {
                httpContext.Response.StatusCode = 203;
            }
            return Task.CompletedTask;
        };

        public static void UseHealthCheck(this IApplicationBuilder app, string path = "/healthz")
        {
            if (app == null)
                throw new ArgumentNullException(nameof(app));
            var handler = new RouteHandler(_HealthCheckDelegate);
            var routeBuilder = new RouteBuilder(app, handler);
            routeBuilder.MapRoute("Health Check Route", path);
            var router = routeBuilder.Build();
            app.UseRouter(router);
        }
    }
}