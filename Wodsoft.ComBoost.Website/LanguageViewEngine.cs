using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wodsoft.ComBoost.Website
{
    public class LanguageViewEngine : System.Web.Mvc.RazorViewEngine
    {
        protected override System.Web.Mvc.IView CreateView(System.Web.Mvc.ControllerContext controllerContext, string viewPath, string masterPath)
        {
            if (controllerContext.RouteData.Values["lang"] != null)
                viewPath = viewPath.Insert(7, "/" + (string)controllerContext.RouteData.Values["lang"]);
            return base.CreateView(controllerContext, viewPath, masterPath);
        }

        protected override bool FileExists(System.Web.Mvc.ControllerContext controllerContext, string virtualPath)
        {
            if (controllerContext.RouteData.Values["lang"] != null)
                virtualPath = virtualPath.Insert(7, "/" + (string)controllerContext.RouteData.Values["lang"]);
            return base.FileExists(controllerContext, virtualPath);
        }
    }
}