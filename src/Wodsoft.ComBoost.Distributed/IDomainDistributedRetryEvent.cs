using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost
{
    [DomainDistributedEventFeature(DomainDistributedEventFeatures.Retry)]
    public interface IDomainDistributedRetryEvent : IDomainDistributedHandleOnceEvent
    {
        int RetryCount { get; set; }
    }
}
