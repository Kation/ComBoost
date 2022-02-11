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
        private static RequestDelegate DomainServiceDelegate = async httpContext =>
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
            endpoint.Map("/{service}/{method}", DomainServiceDelegate);
        }
#endif

        public static void UseDomainService(this IApplicationBuilder app)
        {
            if (app == null)
                throw new ArgumentNullException(nameof(app));
            var handler = new RouteHandler(DomainServiceDelegate);
            var routeBuilder = new RouteBuilder(app, handler);
            routeBuilder.MapRoute("Domain Service Route", "{service}/{method}");
            var router = routeBuilder.Build();
            app.UseRouter(router);
        }
    }
}