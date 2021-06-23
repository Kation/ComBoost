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
            builder.Services.AddSingleton<IDomainDistributedEventProvider, DomainRabbitMQEventProvider>();
            return builder;
        }

        public static IComBoostDistributedBuilder UseRabbitMQ(this IComBoostDistributedBuilder builder, Action<DomainRabbitMQOptions> optionsConfigure)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            if (optionsConfigure == null)
                throw new ArgumentNullException(nameof(optionsConfigure));
            builder.Services.PostConfigure(optionsConfigure);
            builder.Services.AddSingleton<IDomainRabbitMQProvider, DomainRabbitMQProvider>();
            builder.Services.AddSingleton<IDomainDistributedEventProvider, DomainRabbitMQEventProvider>();
            return builder;
        }

        public static IComBoostDistributedBuilder UseRabbitMQ(this IComBoostDistributedBuilder builder, string connectionString)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            if (connectionString == null)
                throw new ArgumentNullException(nameof(connectionString));
            builder.Services.PostConfigure<DomainRabbitMQOptions>(options => options.ConnectionString = connectionString);
            builder.Services.AddSingleton<IDomainRabbitMQProvider, DomainRabbitMQProvider>();
            builder.Services.AddSingleton<IDomainDistributedEventProvider, DomainRabbitMQEventProvider>();
            return builder;
        }
    }
}
