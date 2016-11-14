using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Security;
using Microsoft.AspNetCore.Routing;

namespace Microsoft.AspNetCore.Builder
{
    public static class AppBuilderExtensions
    {
        public static void UseComBoostMvcAuthentication(this IApplicationBuilder app)
        {
            app.UseComBoostAuthentication();
            UseComBoostMvcAuthentication(app, new ComBoostAuthenticationOptions());
        }

        public static void UseComBoostMvcAuthentication(this IApplicationBuilder app, ComBoostAuthenticationOptions options)
        {
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
        }
    }
}
