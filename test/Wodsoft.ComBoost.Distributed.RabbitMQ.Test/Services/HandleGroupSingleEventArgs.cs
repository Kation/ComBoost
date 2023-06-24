using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Distributed.RabbitMQ.Test.Services
{
    public class HandleGroupSingleEventArgs : DomainServiceEventArgs, IDomainDistributedSingleHandlerEvent, IDomainDistributedMustHandleEvent, IDomainDistributedGroupEvent
    {
        public string Text { get; set; }
    }
}
