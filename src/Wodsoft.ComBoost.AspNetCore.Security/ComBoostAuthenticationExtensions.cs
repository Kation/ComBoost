using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Security;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Builder
{
    public static class ComBoostAuthenticationExtensions
    {
        public static void UseComBoostAuthentication(this IApplicationBuilder app)
        {
            if (app == null)
                throw new ArgumentNullException(nameof(app));
            var options = app.ApplicationServices.GetRequiredService<ComBoostAuthenticationOptions>();
            app.UseMiddleware<ComBoostAuthenticationMiddleware>(new Microsoft.Extensions.Options.OptionsWrapper<ComBoostAuthenticationOptions>(options));
            app.UseClaimsTransformation(new ClaimsTransformationOptions
            {
                Transformer = new ComBoostAuthenticationTransformer()
            });
        }
    }
}

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ComBoostAuthenticationExtensions
    {
        public static void AddComBoostAuthentication(this IServiceCollection serviceCollection)
        {
            if (serviceCollection == null)
                throw new ArgumentNullException(nameof(serviceCollection));
            AddComBoostAuthentication(serviceCollection, new ComBoostAuthenticationOptions());
        }

        public static void AddComBoostAuthentication(this IServiceCollection serviceCollection, ComBoostAuthenticationOptions options)
        {
            if (serviceCollection == null)
                throw new ArgumentNullException(nameof(serviceCollection));
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            serviceCollection.AddSingleton(options);
        }
    }
}
