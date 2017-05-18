using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Wodsoft.ComBoost.AspNetCore
{
    public class HttpFormFileValueSelector : HttpValueSelector
    {
        public HttpFormFileValueSelector(HttpContext httpContext) : base(httpContext)
        { }

        protected override string[] GetKeysCore()
        {
            return HttpContext.Request.Form.Files.Where(t => !string.IsNullOrEmpty(t.FileName)).Select(t => t.Name).ToArray();
        }

        protected override object GetValueCore(string key)
        {
            return HttpContext.Request.Form.Files.GetFiles(key);
        }
    }
}
