using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Wodsoft.ComBoost;

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

        public static IComBoostBuilder AddEmptyContextProvider(this IComBoostBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            builder.Services.AddSingleton<IDomainContextProvider, EmptyDomainContextProvider>();
            return builder;
        }

        public static IComBoostBuilder AddDomainGlobalFilter<T>(this IComBoostBuilder builder)
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
