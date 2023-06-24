using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost
{
    [DomainDistributedEventFeature(DomainDistributedEventFeatures.SignelHandler)]
    public interface IDomainDistributedSingleHandlerEvent : IDomainDistributedHandleOnceEvent
    {
    }
}
