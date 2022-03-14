using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost.Distributed.RabbitMQ
{
    public class DomainRabbitMQLogState
    {
        public DomainRabbitMQLogState(string eventType, string exchange, bool redelivered, ulong deliveryTag)
        {
            EventType = eventType;
            Exchange = exchange;
            Redelivered = redelivered;
            DeliveryTag = deliveryTag;
        }

        public bool Redelivered { get; }

        public string Exchange { get; }

        public ulong DeliveryTag { get; }

        public string EventType { get; }
    }
}
