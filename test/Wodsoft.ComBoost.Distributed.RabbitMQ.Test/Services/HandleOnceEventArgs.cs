using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Distributed.RabbitMQ.Test.Services
{
    public class HandleOnceEventArgs : DomainServiceEventArgs, IDomainDistributedHandleOnceEvent, IDomainDistributedMustHandleEvent
    {
        public string Text { get; set; }
    }
}
