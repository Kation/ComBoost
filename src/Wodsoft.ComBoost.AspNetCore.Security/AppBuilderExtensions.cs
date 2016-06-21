using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Security;

namespace Microsoft.AspNetCore.Builder
{
    public static class AppBuilderExtensions
    {
        public static void UseComBoostAuthentication(this IApplicationBuilder app)
        {
            UseComBoostAuthentication(app, new ComBoostAuthenticationOptions());
        }

        public static void UseComBoostAuthentication(this IApplicationBuilder app, ComBoostAuthenticationOptions options)
        {
            app.UseMiddleware<ComBoostAuthenticationMiddleware>(new Microsoft.Extensions.Options.OptionsWrapper<ComBoostAuthenticationOptions>(options));
            app.UseClaimsTransformation(new ClaimsTransformationOptions
            {
                Transformer = new ComBoostAuthenticationTransformer()
            });
        }
    }
}
