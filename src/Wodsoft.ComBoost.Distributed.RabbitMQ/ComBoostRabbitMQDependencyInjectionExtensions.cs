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
        public static IComBoostDistributedEventProviderBuilder UseRabbitMQ(this IComBoostDistributedBuilder builder, Action<DomainRabbitMQOptions> optionsConfigure)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            if (optionsConfigure == null)
                throw new ArgumentNullException(nameof(optionsConfigure));
            DomainRabbitMQOptions options = new DomainRabbitMQOptions();
            optionsConfigure(options);
            return builder.UseEventProvider<DomainRabbitMQEventProvider>(options);
            //builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IHealthStateProvider, DomainRabbitMQProvider>(sp => sp.GetService<DomainRabbitMQProvider>()));
        }

        public static IComBoostDistributedEventProviderBuilder UseRabbitMQ(this IComBoostDistributedBuilder builder, string connectionString)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            if (connectionString == null)
                throw new ArgumentNullException(nameof(connectionString));
            DomainRabbitMQOptions options = new DomainRabbitMQOptions();
            options.ConnectionString = connectionString;
            return builder.UseEventProvider<DomainRabbitMQEventProvider>(options);
            //builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IHealthStateProvider, DomainRabbitMQProvider>(sp => sp.GetService<DomainRabbitMQProvider>()));
        }
    }
}
