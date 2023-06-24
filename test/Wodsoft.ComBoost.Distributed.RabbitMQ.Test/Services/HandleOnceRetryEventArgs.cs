using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Distributed.RabbitMQ.Test.Services
{
    [DomainDistributedEventRetryTimes(5000, 10000, 15000)]
    public class HandleOnceRetryEventArgs : DomainServiceEventArgs, IDomainDistributedMustHandleEvent, IDomainDistributedRetryEvent, IDomainDistributedHandleOnceEvent
    {
        public string Text { get; set; }

        public int RetryCount { get; set; }
    }
}
