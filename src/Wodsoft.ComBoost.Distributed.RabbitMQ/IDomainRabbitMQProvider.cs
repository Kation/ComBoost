using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost.Distributed.RabbitMQ
{
    public interface IDomainRabbitMQProvider
    {
        IConnection GetConnection();
    }
}
