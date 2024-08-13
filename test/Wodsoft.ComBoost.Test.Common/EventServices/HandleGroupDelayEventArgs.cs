using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Test.EventServices
{
    public class HandleGroupDelayEventArgs : DomainServiceEventArgs, IDomainDistributedGroupEvent, IDomainDistributedDelayEvent, IDomainDistributedMustHandleEvent
    {
        public string Text { get; set; }

        public int Delay { get; set; }
    }
}
