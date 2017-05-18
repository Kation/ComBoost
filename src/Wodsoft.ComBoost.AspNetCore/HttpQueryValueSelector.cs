using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Wodsoft.ComBoost.AspNetCore
{
    public class HttpQueryValueSelector : HttpValueSelector
    {
        public HttpQueryValueSelector(HttpContext httpContext) : base(httpContext)
        { }

        protected override string[] GetKeysCore()
        {
            return HttpContext.Request.Query.Keys.ToArray();
        }

        protected override object GetValueCore(string key)
        {
            return HttpContext.Request.Query[key];
        }
    }
}
