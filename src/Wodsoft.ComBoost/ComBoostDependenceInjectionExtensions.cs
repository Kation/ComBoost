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
            services.AddScoped<IAuthorizationProvider, DefaultAuthorizationProvider>();
            services.PostConfigure<AuthenticationProviderOptions>(options => options.AddHandler<AnonymousAuthenticationHandler>(1000));
            return new ComBoostBuilder(services);
        }

        public static IComBoostBuilder AddAuthorizationProvider<T>(this IComBoostBuilder builder)
            where T : class, IAuthorizationProvider
        {
            builder.Services.AddScoped<IAuthorizationProvider, T>();
            return builder;
        }

        public static IComBoostBuilder AddLocalService(this IComBoostBuilder builder, Action<IComBoostLocalBuilder> localBuilderConfigure)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            var localBuilder = new ComBoostLocalBuilder(builder.Services, builder);
            foreach (var module in builder.Modules)
                module.ConfigureDomainServices(localBuilder);
            localBuilderConfigure(localBuilder);
            return builder;
        }

        public static IComBoostBuilder ClearDomainContextProvider(this IComBoostBuilder builder)
        {
            builder.Services.PostConfigure<CompositeDomainContextProviderOptions>(options => options.ClearContextProvider());
            return builder;
        }

        public static IComBoostBuilder AddEmptyContextProvider(this IComBoostBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            builder.Services.PostConfigure<CompositeDomainContextProviderOptions>(options => options.TryAddContextProvider<EmptyDomainContextProvider>(1000));
            return builder;
        }

        public static IComBoostBuilder TryAddContextProvider<TProvider>(this IComBoostBuilder builder, int order)
            where TProvider : class, IDomainContextProvider
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            builder.Services.PostConfigure<CompositeDomainContextProviderOptions>(options => options.TryAddContextProvider<TProvider>(order));
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

        private static readonly MethodInfo _AddServiceMethod = typeof(IComBoostLocalBuilder).GetMethod("AddService", BindingFlags.Public | BindingFlags.Instance);
        public static IComBoostLocalBuilder AddServiceFromAssembly(this IComBoostLocalBuilder builder, Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (type.GetInterfaces().Any(t => t == typeof(IDomainService)))
                    _AddServiceMethod.MakeGenericMethod(type).Invoke(builder, null);
            }
            return builder;
        }

        public static IServiceCollection AddInMemorySemaphoreProvider(this IServiceCollection services)
        {
            return services.AddSingleton<ISemaphoreProvider, InMemorySemaphoreProvider>();
        }
    }
}
