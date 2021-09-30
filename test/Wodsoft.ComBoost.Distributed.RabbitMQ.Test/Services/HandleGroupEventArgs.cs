using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Distributed.RabbitMQ.Test.Services
{
    public class HandleGroupEventArgs : DomainServiceEventArgs, IDomainDistributedGroupEvent, IDomainDistributedMustHandleEvent
    {
        public string Text { get; set; }
    }
}
