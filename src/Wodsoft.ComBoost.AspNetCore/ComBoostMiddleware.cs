using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.AspNetCore
{
    public class ComBoostMiddleware
    {
        private RequestDelegate _Next;

        public ComBoostMiddleware(RequestDelegate next)
        {
            _Next = next;
        }

        public Task Invoke(HttpContext httpContext)
        {
            if (SynchronizationContext.Current == null)
            {
                var context = new SynchronizationContext();
                SynchronizationContext.SetSynchronizationContext(context);
            }
            return _Next(httpContext);
        }
    }
}

namespace Microsoft.AspNetCore.Builder
{
    public static class ComBoostMiddlewareExtensions
    {
        public static void UseComBoost(this IApplicationBuilder app)
        {
            if (app == null)
                throw new ArgumentNullException(nameof(app));            
            app.UseMiddleware<Wodsoft.ComBoost.AspNetCore.ComBoostMiddleware>();
        }
    }
}