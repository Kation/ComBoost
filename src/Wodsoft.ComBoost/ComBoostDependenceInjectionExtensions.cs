using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Wodsoft.ComBoost;
using Wodsoft.ComBoost.Mock;
using Wodsoft.ComBoost.Security;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ComBoostDependenceInjectionExtensions
    {
        public static IComBoostBuilder AddComBoost(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            services.AddOptions();
            services.TryAddSingleton<IDomainServiceEventManager>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<DomainServiceEventManagerOptions>>();
                var manager = new DomainServiceEventManager(options.Value);
                return manager;
            });
            services.TryAddScoped<IDomainContextProvider, CompositeDomainContextProvider>();
            services.TryAddScoped<IAuthenticationProvider, AuthenticationProvider>();
            services.PostConfigure<AuthenticationProviderOptions>(options => options.AddHandler<AnonymousAuthenticationHandler>(1000));
            return new ComBoostBuilder(services);
        }

        public static IComBoostBuilder AddLocalService(this IComBoostBuilder builder, Action<IComBoostLocalBuilder> localBuilderConfigure = null)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            if (localBuilderConfigure != null)
                localBuilderConfigure(new ComBoostLocalBuilder(builder.Services));
            return builder;
        }

        [Obsolete("It is already add a empty context provider when use AddComBoost.")]
        public static IComBoostBuilder AddEmptyContextProvider(this IComBoostBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            builder.Services.PostConfigure<CompositeDomainContextProviderOptions>(options => options.AddContextProvider<EmptyDomainContextProvider>(1000));
            return builder;
        }

        public static IComBoostBuilder AddGlobalFilter<T>(this IComBoostBuilder builder)
            where T : class, IDomainServiceFilter, new()
        {
            builder.Services.PostConfigure<DomainFilterOptions>(options =>
            {
                options.Add(new T());
            });
            return builder;
        }

        public static IComBoostLocalBuilder AddEventHandlersFromAssembly(this IComBoostLocalBuilder builder, Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (type.GetConstructor(Array.Empty<Type>()) == null)
                    continue;
                foreach (var item in type.GetInterfaces())
                {
                    if (item.IsGenericType && item.GetGenericTypeDefinition() == typeof(IDomainServiceEventHandler<>))
                    {
                        typeof(IComBoostLocalBuilder).GetMethod(nameof(IComBoostLocalBuilder.AddEventHandler)).MakeGenericMethod(item, item.GetGenericArguments()[0]).Invoke(builder, Array.Empty<object>());
                    }
                }
            }
            return builder;
        }
    }
}
