using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Wodsoft.ComBoost.AspNetCore
{
    public class HttpQueryValueSelector : HttpStringValuesSelector
    {
        public HttpQueryValueSelector(HttpContext httpContext) : base(httpContext)
        { }

        protected override string[] GetKeysCore()
        {
            return HttpContext.Request.Query.Keys.ToArray();
        }

        protected override StringValues GetStringValue(string key)
        {
            return HttpContext.Request.Query[key];
        }
    }
}
