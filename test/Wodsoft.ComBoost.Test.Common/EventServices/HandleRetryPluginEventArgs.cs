using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Test.EventServices
{
    [DomainDistributedEventRetryTimes(5000, 10000, 15000)]
    public class HandleRetryPluginEventArgs : DomainServiceEventArgs, IDomainDistributedMustHandleEvent, IDomainDistributedRetryEvent
    {
        public string Text { get; set; }

        public int RetryCount { get; set; }
    }
}
