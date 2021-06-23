using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost.Distributed.RabbitMQ
{
    public class DomainRabbitMQEventOptions
    {
        public string Prefix { get; set; }

        public ushort PrefetchCount { get; set; } = 10;
    }
}
