using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using Wodsoft.ComBoost;
using Wodsoft.ComBoost.AspNetCore;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DomainAspNetCoreDependencyInjectionExtensions
    {
        public static void AddDomainServiceMapping<T>(this IComBoostAspNetCoreBuilder builder, string name = null)
            where T : class, IDomainService
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            if (name == null)
                name = typeof(T).Name;
            builder.Services.Configure<DomainServiceMapping>(name.ToLower(), options =>
            {
                options.ServiceType = typeof(T);
            });
            builder.Services.AddSingleton<DomainServiceDescriptor<T>>();
            builder.Services.AddTransient<T>();
        }

        public static IComBoostBuilder AddAspNetCore(this IComBoostBuilder builder, Action<IComBoostAspNetCoreBuilder> builderConfigure = null)
        {
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<IDomainContextProvider, HttpDomainContextProvider>();
            if (builderConfigure != null)
                builderConfigure(new ComBoostAspNetCoreBuilder(builder.Services));
            return builder;
        }
    }
}
