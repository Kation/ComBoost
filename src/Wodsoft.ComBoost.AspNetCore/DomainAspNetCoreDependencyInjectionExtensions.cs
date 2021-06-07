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
        public static void AddDomainServiceMapping<T>(this IServiceCollection services, string name)
            where T : class, IDomainService
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            services.Configure<DomainServiceMapping>(name.ToLower(), options =>
            {
                options.ServiceType = typeof(T);
            });
            services.AddSingleton<DomainServiceDescriptor<T>>();
            services.AddTransient<T>();
        }

        //public static void AddComBoostAspNetCore(this IServiceCollection services)
        //{
        //    services.AddSingleton<IExecutionResultHandler, DefaultExecutionResultHandler>();
        //}

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
