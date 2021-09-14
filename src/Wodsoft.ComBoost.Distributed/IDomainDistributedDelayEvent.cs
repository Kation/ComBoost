using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost
{
    [DomainDistributedEventFeature("DELAY")]
    public interface IDomainDistributedDelayEvent
    {
        int Delay { get; set; }
    }
}
