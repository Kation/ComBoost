using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private static RequestDelegate _DomainServiceDelegate = async httpContext =>
        {
            var service = (string?)httpContext.GetRouteValue("service");
            var method = (string?)httpContext.GetRouteValue("method");
            if (string.IsNullOrWhiteSpace(service) || string.IsNullOrWhiteSpace(method))
            {
                httpContext.Response.StatusCode = 404;
                return;
            }
            service = service.ToLower();

            var optionsFactory = httpContext.RequestServices.GetRequiredService<IOptionsFactory<DomainServiceMapping>>();
            var mapping = optionsFactory.Create(service);
            if (mapping == null)
            {
                httpContext.Response.StatusCode = 404;
                await httpContext.Response.WriteAsync($"无法找到对应的领域服务。");
                return;
            }
            if (mapping.ServiceType == null)
                throw new InvalidOperationException("领域服务映射配置没有配置服务类型。");
            IDomainService domainService = (IDomainService)ActivatorUtilities.CreateInstance(httpContext.RequestServices, mapping.ServiceType);

            HttpDomainContext context = new HttpDomainContext(httpContext);

            var descriptor = (DomainServiceDescriptor)httpContext.RequestServices.GetRequiredService(typeof(DomainServiceDescriptor<>).MakeGenericType(mapping.ServiceType));
            if (descriptor.ContainsMethod(method))
            {
                var executionContext = await descriptor.ExecuteAsync(method, context);
                var resultHandler = httpContext.RequestServices.GetRequiredService<IExecutionResultHandler>();
                await resultHandler.Handle(executionContext, httpContext);
            }
            else
            {
                httpContext.Response.StatusCode = 404;
                await httpContext.Response.WriteAsync($"领域服务“{mapping.ServiceType.FullName}”不存在方法“{method}”。");
            }
        };

#if !NETCOREAPP2_1
        public static void MapDomainService(this IEndpointRouteBuilder endpoint)
        {
            if (endpoint == null)
                throw new ArgumentNullException(nameof(endpoint));
            endpoint.Map("/{service}/{method}", _DomainServiceDelegate);
        }
        
        public static void MapHealthCheck(this IEndpointRouteBuilder endpoint, string path)
        {
            if (endpoint == null)
                throw new ArgumentNullException(nameof(endpoint));
            endpoint.Map(path, _HealthCheckDelegate);
        }
#endif

        public static void UseDomainService(this IApplicationBuilder app)
        {
            if (app == null)
                throw new ArgumentNullException(nameof(app));
            var handler = new RouteHandler(_DomainServiceDelegate);
            var routeBuilder = new RouteBuilder(app, handler);
            routeBuilder.MapRoute("Domain Service Route", "{service}/{method}");
            var router = routeBuilder.Build();
            app.UseRouter(router);
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