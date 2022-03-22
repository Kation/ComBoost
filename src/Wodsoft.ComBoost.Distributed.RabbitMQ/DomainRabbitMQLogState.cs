using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost.Distributed.RabbitMQ
{
    public class DomainRabbitMQLogState : IEnumerable<KeyValuePair<string, object>>
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

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            yield return new KeyValuePair<string, object>("RabbitMQRedelivered", Redelivered);
            yield return new KeyValuePair<string, object>("RabbitMQExchange", Exchange);
            yield return new KeyValuePair<string, object>("RabbitMQDeliveryTag", DeliveryTag);
            yield return new KeyValuePair<string, object>("RabbitMQEventType", EventType);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
