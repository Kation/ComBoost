using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost.Distributed.RabbitMQ
{
    public class DomainRabbitMQOptions
    {
        public string ConnectionString { get; set; }

        public string HostName { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string VirtualHost { get; set; }

        public int Port { get; set; }

        public Action<IConnectionFactory> FactoryConfigure { get; set; }
    }
}
