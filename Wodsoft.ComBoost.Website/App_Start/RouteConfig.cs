using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Wodsoft.ComBoost.Website
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("Home", "", new { controller = "Home", action = "Init" });

            routes.MapRoute("License", "license", new { controller = "Home", action = "License" });

            routes.MapRoute(
                name: "Default",
                url: "{lang}/{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}