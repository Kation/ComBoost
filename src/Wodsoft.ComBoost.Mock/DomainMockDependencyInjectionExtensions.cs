using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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
    public static class DomainMockDependencyInjectionExtensions
    {
        public static IComBoostBuilder AddMock(this IComBoostBuilder builder, Action<IComBoostMockBuilder>? builderConfigure = null)
        {
            builder.Services.AddInMemorySemaphoreProvider();
            builder.Services.AddScoped<IDomainContextProvider, MockDomainContextProvider>();
            builder.Services.AddScoped<MockAuthenticationSettings>();
            builder.Services.PostConfigure<AuthenticationProviderOptions>(options =>
            {
                options.AddHandler<MockAuthenticationHandler>();
            });
            if (builderConfigure != null)
            {
                builderConfigure(new ComBoostMockBuilder(builder.Services));
            }
            builder.Services.AddLogging(builder =>
            {
                builder.AddDebug();
            });
            return builder;
        }

        [Obsolete]
        public static IComBoostBuilder AddMockService(this IComBoostBuilder builder, Func<IMock> mockGetter, Action<IComBoostMockServiceBuilder> builderConfigure)
        {
            if (builderConfigure == null)
                throw new ArgumentNullException(nameof(builderConfigure));
            builderConfigure(new ComBoostMockServiceBuilder(builder.Services, () => mockGetter().ServiceProvider));
            return builder;
        }

        public static IComBoostBuilder AddMockService(this IComBoostBuilder builder, Func<IHost> hostGetter, Action<IComBoostMockServiceBuilder> builderConfigure)
        {
            if (builderConfigure == null)
                throw new ArgumentNullException(nameof(builderConfigure));
            builderConfigure(new ComBoostMockServiceBuilder(builder.Services, () => hostGetter().Services));
            return builder;
        }

        public static IComBoostDistributedEventProviderBuilder UseInMemory(this IComBoostDistributedBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            return builder.UseEventProvider<MockInMemoryEventProvider>(new MockInMemoryEventOptions());
        }

        public static IComBoostDistributedEventProviderBuilder UseInMemory(this IComBoostDistributedBuilder builder, Action<MockInMemoryEventOptions> optionsConfigure)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            if (optionsConfigure == null)
                throw new ArgumentNullException(nameof(optionsConfigure));
            var options = new MockInMemoryEventOptions();
            optionsConfigure(options);
            return builder.UseEventProvider<MockInMemoryEventProvider>(options);
        }

        public static IComBoostDistributedEventProviderBuilder UseInMemory(this IComBoostDistributedBuilder builder, object instanceKey)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            var options = new MockInMemoryEventOptions();
            options.InstanceKey = instanceKey;
            return builder.UseEventProvider<MockInMemoryEventProvider>(options);
        }

        private readonly static MethodInfo _AddServiceMethod = typeof(IComBoostMockServiceBuilder).GetMethod("AddService");
        public static IComBoostMockServiceBuilder AddServiceInAssembly(this IComBoostMockServiceBuilder builder, string serviceName, Assembly assembly)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            if (serviceName == null)
                throw new ArgumentNullException(nameof(serviceName));
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));
            foreach (var type in assembly.GetTypes())
            {
                if (type.IsInterface && !type.IsGenericTypeDefinition && type.GetInterfaces().Any(t => t == typeof(IDomainTemplate)))
                {
                    var attr = type.GetCustomAttribute<DomainDistributedServiceAttribute>();
                    if (attr != null && attr.ServiceName == serviceName)
                        _AddServiceMethod.MakeGenericMethod(type).Invoke(builder, Array.Empty<object>());
                }
            }
            return builder;
        }

        public static IComBoostMockServiceBuilder AddServiceInAssembly(this IComBoostMockServiceBuilder builder, Assembly assembly)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));
            foreach (var type in assembly.GetTypes())
            {
                if (type.IsInterface && !type.IsGenericTypeDefinition && type.GetInterfaces().Any(t => t == typeof(IDomainTemplate)))
                {
                    _AddServiceMethod.MakeGenericMethod(type).Invoke(builder, Array.Empty<object>());
                }
            }
            return builder;
        }
    }
}
