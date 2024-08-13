using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost
{
    [DomainDistributedEventFeature(DomainDistributedEventFeatures.Delay)]
    public interface IDomainDistributedDelayEvent
    {
        int Delay { get; set; }
    }
}
