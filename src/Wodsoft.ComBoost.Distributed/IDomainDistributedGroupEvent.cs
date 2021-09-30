using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost
{
    [DomainDistributedEventFeature(DomainDistributedEventFeatures.Group)]
    public interface IDomainDistributedGroupEvent : IDomainDistributedHandleOnceEvent
    {
    }
}
