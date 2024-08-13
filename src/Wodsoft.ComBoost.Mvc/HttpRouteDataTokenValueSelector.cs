using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Wodsoft.ComBoost.AspNetCore;

namespace Wodsoft.ComBoost.Mvc
{
    public class HttpRouteDataTokenValueSelector : HttpValueSelector
    {
        public HttpRouteDataTokenValueSelector(HttpContext httpContext) : base(httpContext)
        { }

        protected override string[] GetKeysCore()
        {
            return HttpContext.GetRouteData().DataTokens.Keys.ToArray();
        }

        protected override object? GetValueCore(string key)
        {
            return HttpContext.GetRouteData().DataTokens[key];
        }
    }
}
