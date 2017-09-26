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
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ComBoostMvcExtensions
    {
        public static void AddComBoostMvcOptions(this MvcOptions options)
        {
            options.ModelBinderProviders.Add(new SelectedFileBinderProvider());
        }

        public static void AddComBoostMvcAuthentication<T>(this IServiceCollection services)
            where T : AuthenticationHandler<ComBoostAuthenticationOptions>
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            AddComBoostMvcAuthentication<T>(services, null);
        }

        public static void AddComBoostMvcAuthentication<T>(this IServiceCollection services, Action<ComBoostAuthenticationOptions> configueOptions)
            where T : AuthenticationHandler<ComBoostAuthenticationOptions>
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            services.AddComBoostAuthentication<T>(options =>
            {
                var oldLoginPath = options.LoginPath;
                options.LoginPath = context =>
                {
                    var routeData = context.GetRouteData();
                    var path = routeData.DataTokens["loginPath"] as string;
                    return path ?? oldLoginPath(context);
                };
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
                configueOptions?.Invoke(options);
            });
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
            app.UseAuthentication();
            app.UseMvc(configureRoutes);
        }
    }
}
