using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Test.EventServices
{
    public class HandleMoreEventArgs : DomainServiceEventArgs, IDomainDistributedMustHandleEvent
    {
        public string Text { get; set; }
    }
}
