using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Security;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.AspNetCore.Builder
{
    public static class ComBoostAuthenticationExtensions
    {
        //public static void UseComBoostAuthentication(this IApplicationBuilder app)
        //{
        //    if (app == null)
        //        throw new ArgumentNullException(nameof(app));
        //    var options = app.ApplicationServices.GetRequiredService<ComBoostAuthenticationOptions>();
        //    app.UseMiddleware<ComBoostAuthenticationMiddleware>(new Microsoft.Extensions.Options.OptionsWrapper<ComBoostAuthenticationOptions>(options));
        //    app.UseClaimsTransformation(new ClaimsTransformationOptions
        //    {
        //        Transformer = new ComBoostAuthenticationTransformer()
        //    });
        //}
    }
}

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ComBoostAuthenticationExtensions
    {
        public static void AddComBoostAuthentication<T>(this IServiceCollection services)
            where T : AuthenticationHandler<ComBoostAuthenticationOptions>
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            AddComBoostAuthentication<T>(services, null);
        }

        public static void AddComBoostAuthentication<T>(this IServiceCollection services, Action<ComBoostAuthenticationOptions> configureOptions)
            where T : AuthenticationHandler<ComBoostAuthenticationOptions>
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            services.AddScoped<IClaimsTransformation, ComBoostAuthenticationTransformer>();
            var builder = services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = "ComBoost";
                o.DefaultChallengeScheme = "ComBoost";
            });
            builder.AddScheme<ComBoostAuthenticationOptions, T>("ComBoost", "ComBoost", configureOptions);
            builder.Services.Configure(configureOptions);
            //builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<ComBoostAuthenticationOptions>, PostConfigureComBoostAuthenticationOptions>());
            services.AddSingleton<IPostConfigureOptions<ComBoostAuthenticationOptions>, PostConfigureComBoostAuthenticationOptions>();
        }
    }
}
