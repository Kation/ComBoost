using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Distributed.CAP.Test.Services
{
    public class HandleGroupEventArgs : DomainServiceEventArgs, IDomainDistributedGroupEvent
    {
        public string Text { get; set; }
    }
}
