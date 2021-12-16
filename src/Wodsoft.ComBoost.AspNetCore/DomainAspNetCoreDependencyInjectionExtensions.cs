using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using Wodsoft.ComBoost;
using Wodsoft.ComBoost.AspNetCore;
using Wodsoft.ComBoost.Security;

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
            {
                name = DomainService.GetServiceName(typeof(T));
            }
            builder.Services.Configure<DomainServiceMapping>(name.ToLower(), options =>
            {
                options.ServiceType = typeof(T);
            });
            builder.Services.AddSingleton<DomainServiceDescriptor<T>>();
            builder.Services.AddTransient<T>();
        }

        public static IComBoostBuilder AddAspNetCore(this IComBoostBuilder builder, Action<IComBoostAspNetCoreBuilder> builderConfigure = null)
        {
            builder.Services.AddRouting();
            builder.Services.AddHttpContextAccessor();
            builder.Services.PostConfigure<CompositeDomainContextProviderOptions>(options => options.TryAddContextProvider<HttpDomainContextProvider>(500));
            builder.Services.TryAddScoped<IExecutionResultHandler, DefaultExecutionResultHandler>();
            builder.Services.PostConfigure<AuthenticationProviderOptions>(options => options.AddHandler<AspNetCoreAuthenticationHandler>(500));
            if (builderConfigure != null)
                builderConfigure(new ComBoostAspNetCoreBuilder(builder.Services));
            return builder;
        }
    }
}
