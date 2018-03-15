using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Wodsoft.ComBoost.AspNetCore
{
    /// <summary>
    /// Http头部值选择器。
    /// </summary>
    public class HttpHeaderValueSelector : HttpStringValuesSelector
    {
        public HttpHeaderValueSelector(HttpContext httpContext) : base(httpContext)
        { }

        protected override string[] GetKeysCore()
        {
            return HttpContext.Request.Headers.Keys.ToArray();
        }

        protected override StringValues GetStringValue(string key)
        {
            return HttpContext.Request.Headers[key];
        }
    }
}
