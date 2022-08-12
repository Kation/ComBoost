using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost.Distributed.RabbitMQ
{
    public class DomainRabbitMQOptions
    {
        public string? ConnectionString { get; set; }

        public string? HostName { get; set; }

        public string? UserName { get; set; }

        public string? Password { get; set; }

        public string? VirtualHost { get; set; }

        public int Port { get; set; }

        public string? Prefix { get; set; }

        public ushort PrefetchCount { get; set; } = 10;

        public bool UseQuorum { get; set; } = true;

        public Action<IConnectionFactory>? FactoryConfigure { get; set; }

        public bool IsCriticalHealthState { get; set; } = true;
    }
}
