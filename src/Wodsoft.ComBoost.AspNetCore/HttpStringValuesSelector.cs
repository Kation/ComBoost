using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Wodsoft.ComBoost.AspNetCore
{
    public abstract class HttpStringValuesSelector : HttpValueSelector
    {
        public HttpStringValuesSelector(HttpContext httpContext) : base(httpContext)
        {
        }

        protected sealed override object GetValueCore(string key)
        {
            var value = GetStringValue(key);
            if (value == StringValues.Empty)
                return null;
            return value;
        }

        protected abstract StringValues GetStringValue(string key);
    }
}
