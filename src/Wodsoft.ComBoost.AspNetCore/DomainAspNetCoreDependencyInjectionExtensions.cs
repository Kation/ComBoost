using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using Wodsoft.ComBoost;
using Wodsoft.ComBoost.AspNetCore;
using Wodsoft.ComBoost.Security;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DomainAspNetCoreDependencyInjectionExtensions
    {
        public static IComBoostAspNetCoreBuilder AddDomainEndpoint(this IComBoostAspNetCoreBuilder builder)
        {
            var types = Assembly.GetCallingAssembly().GetTypes().Where(t => !t.IsValueType && !t.IsAbstract && t.GetCustomAttribute<DomainEndpointAttribute>() != null).ToList();
            builder.Services.PostConfigure<DomainEndpointOptions>(options => options.Types.AddRange(types));
#if !NETCOREAPP2_1
            foreach (var type in types)
            {
                builder.Services.AddSingleton(typeof(DomainEndpointApiDescriptorOptions<>).MakeGenericType(type));
                builder.Services.TryAddEnumerable(ServiceDescriptor.Transient(typeof(Microsoft.AspNetCore.Mvc.ApiExplorer.IApiDescriptionProvider), typeof(DomainEndpointApiDescriptorProvider<>).MakeGenericType(type)));
            }
#endif
            return builder;
        }

        public static IComBoostAspNetCoreBuilder AddDomainEndpoint(this IComBoostAspNetCoreBuilder builder, Assembly assembly)
        {
            var types = assembly.GetTypes().Where(t => !t.IsValueType && !t.IsAbstract && t.GetCustomAttribute<DomainEndpointAttribute>() != null).ToList();
            builder.Services.PostConfigure<DomainEndpointOptions>(options => options.Types.AddRange(types));
#if !NETCOREAPP2_1
            foreach (var type in types)
            {
                builder.Services.AddSingleton(typeof(DomainEndpointApiDescriptorOptions<>).MakeGenericType(type));
                builder.Services.TryAddEnumerable(ServiceDescriptor.Transient(typeof(Microsoft.AspNetCore.Mvc.ApiExplorer.IApiDescriptionProvider), typeof(DomainEndpointApiDescriptorProvider<>).MakeGenericType(type)));
            }
#endif
            return builder;
        }

        public static IComBoostBuilder AddAspNetCore(this IComBoostBuilder builder, Action<IComBoostAspNetCoreBuilder>? builderConfigure = null)
        {
            builder.Services.AddRouting();
            builder.Services.AddHttpContextAccessor();
            builder.Services.PostConfigure<CompositeDomainContextProviderOptions>(options => options.TryAddContextProvider<HttpDomainContextProvider>(500));
            builder.Services.TryAddScoped<IExecutionResultHandler, DefaultExecutionResultHandler>();
            builder.Services.PostConfigure<AuthenticationProviderOptions>(options => options.AddHandler<AspNetCoreAuthenticationHandler>(500));
            if (builderConfigure != null)
                builderConfigure(new ComBoostAspNetCoreBuilder(builder.Services, builder));
            return builder;
        }
    }
}
