using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Wodsoft.ComBoost.AspNetCore;
using Microsoft.AspNetCore.Routing;

namespace Wodsoft.ComBoost.Mvc
{
    public class HttpRouteValueValueSelector : HttpValueSelector
    {
        public HttpRouteValueValueSelector(HttpContext httpContext) : base(httpContext)
        { }

        protected override string[] GetKeysCore()
        {
            return HttpContext.GetRouteData().Values.Keys.ToArray();
        }

        protected override object GetValueCore(string key)
        {
            return HttpContext.GetRouteValue(key);
        }
    }
}
