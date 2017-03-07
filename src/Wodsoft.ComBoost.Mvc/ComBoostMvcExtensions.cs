using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Security;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.Extensions.DependencyInjection;
using Wodsoft.ComBoost.Mvc;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ComBoostMvcExtensions
    {
        public static void AddComBoostMvcAuthentication(this IServiceCollection serviceCollection)
        {
            if (serviceCollection == null)
                throw new ArgumentNullException(nameof(serviceCollection));
            AddComBoostMvcAuthentication(serviceCollection, new ComBoostAuthenticationOptions());
        }

        public static void AddComBoostMvcAuthentication(this IServiceCollection serviceCollection, ComBoostAuthenticationOptions options)
        {
            if (serviceCollection == null)
                throw new ArgumentNullException(nameof(serviceCollection));
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            var oldCookieName = options.CookieName;
            options.CookieName = context =>
            {
                var cookieName = oldCookieName(context);
                var routeData = context.GetRouteData();
                if (routeData == null)
                    return cookieName;
                var area = routeData.DataTokens["authArea"];
                if (area == null)
                    return cookieName;
                return cookieName + "_" + area;
            };
            var oldLoginPath = options.LoginPath;
            options.LoginPath = context =>
            {
                var routeData = context.GetRouteData();
                var path = routeData.DataTokens["loginPath"] as string;
                return path ?? oldLoginPath(context);
            };
            var oldLogoutPath = options.LogoutPath;
            options.LogoutPath = context =>
            {
                var routeData = context.GetRouteData();
                var path = routeData.DataTokens["logoutPath"] as string;
                return path ?? oldLogoutPath(context);
            };
            var oldExpireTime = options.ExpireTime;
            options.ExpireTime = context =>
            {
                var routeData = context.GetRouteData();
                var path = routeData.DataTokens["expireTime"] as TimeSpan?;
                return path ?? oldExpireTime(context);
            };
            serviceCollection.AddSingleton(options);
        }
    }
}

namespace Microsoft.AspNetCore.Builder
{
    public static class ComBoostMvcExtensions
    {
        public static void UseComBoostMvc(this IApplicationBuilder app, Action<IRouteBuilder> configureRoutes)
        {
            if (app == null)
                throw new ArgumentNullException(nameof(app));

            var routes = new RouteBuilder(app)
            {
                DefaultHandler = app.ApplicationServices.GetRequiredService<MvcRouteHandler>(),
            };
            configureRoutes(routes);
            routes.Routes.Insert(0, AttributeRouting.CreateAttributeMegaRoute(app.ApplicationServices));
            var router = routes.Build();
            app.UseMiddleware<ComBoostMvcMiddleware>(router);
            app.UseComBoostAuthentication();
            app.UseMvc(configureRoutes);
        }
    }
}
