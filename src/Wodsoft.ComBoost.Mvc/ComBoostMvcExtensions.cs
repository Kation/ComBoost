using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Security;
using Microsoft.AspNetCore.Routing;

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
            //options.LoginPath = context =>
            //{
            //    var routeData = context.GetRouteData();
            //    var area = routeData.DataTokens["authArea"];
            //    string path = "/Account/LoginIn";
            //    if (area == null)
            //        return path;
            //    return area + "/" + path;
            //};
            //options.LogoutPath = context =>
            //{
            //    var routeData = context.GetRouteData();
            //    var area = routeData.DataTokens["authArea"];
            //    string path = "/Account/LoginOut";
            //    if (area == null)
            //        return path;
            //    return area + "/" + path;
            //};
            serviceCollection.AddSingleton(options);
        }
    }
}
