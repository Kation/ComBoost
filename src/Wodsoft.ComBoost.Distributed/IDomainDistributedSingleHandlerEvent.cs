using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost
{
    [DomainDistributedEventFeature(DomainDistributedEventFeatures.SingleHandler)]
    public interface IDomainDistributedSingleHandlerEvent : IDomainDistributedHandleOnceEvent
    {
    }
}
