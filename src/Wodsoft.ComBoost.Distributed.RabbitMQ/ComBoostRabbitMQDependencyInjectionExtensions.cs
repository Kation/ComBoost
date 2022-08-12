using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using Wodsoft.ComBoost;
using Wodsoft.ComBoost.Distributed.RabbitMQ;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ComBoostRabbitMQDependencyInjectionExtensions
    {
        public static IComBoostDistributedBuilder UseRabbitMQ(this IComBoostDistributedBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IDomainDistributedEventProvider, DomainRabbitMQEventProvider>());
            return builder;
        }

        public static IComBoostDistributedBuilder UseRabbitMQ(this IComBoostDistributedBuilder builder, Action<DomainRabbitMQOptions> optionsConfigure)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            if (optionsConfigure == null)
                throw new ArgumentNullException(nameof(optionsConfigure));
            builder.Services.PostConfigure(optionsConfigure);
            builder.Services.AddSingleton<DomainRabbitMQProvider>();
            builder.Services.AddSingleton<IDomainRabbitMQProvider>(sp => sp.GetService<DomainRabbitMQProvider>());
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IHealthStateProvider, DomainRabbitMQProvider>(sp => sp.GetService<DomainRabbitMQProvider>()));
            return UseRabbitMQ(builder);
        }

        public static IComBoostDistributedBuilder UseRabbitMQ(this IComBoostDistributedBuilder builder, string connectionString)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            if (connectionString == null)
                throw new ArgumentNullException(nameof(connectionString));
            builder.Services.PostConfigure<DomainRabbitMQOptions>(options => options.ConnectionString = connectionString);
            builder.Services.AddSingleton<DomainRabbitMQProvider>();
            builder.Services.AddSingleton<IDomainRabbitMQProvider>(sp => sp.GetService<DomainRabbitMQProvider>());
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IHealthStateProvider, DomainRabbitMQProvider>(sp => sp.GetService<DomainRabbitMQProvider>()));
            return UseRabbitMQ(builder);
        }
    }
}
