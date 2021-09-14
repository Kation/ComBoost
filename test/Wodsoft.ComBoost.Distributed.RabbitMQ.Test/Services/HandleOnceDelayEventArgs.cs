using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Distributed.RabbitMQ.Test.Services
{
    public class HandleOnceDelayEventArgs : DomainServiceEventArgs, IDomainDistributedHandleOnceEvent, IDomainDistributedMustHandleEvent, IDomainDistributedDelayEvent
    {
        public string Text { get; set; }

        public int Delay { get; set; }
    }
}
