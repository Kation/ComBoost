using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Wodsoft.ComBoost.AspNetCore
{
    public class HttpHeaderValueSelector : HttpValueSelector
    {
        public HttpHeaderValueSelector(HttpContext httpContext) : base(httpContext)
        { }

        protected override string[] GetKeysCore()
        {
            return HttpContext.Request.Headers.Keys.ToArray();
        }

        protected override object GetValueCore(string key)
        {
            return HttpContext.Request.Headers[key];
        }
    }
}
