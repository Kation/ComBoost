using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost.Aggregation.Caching
{
    public class DomainAggregatorDistributedCacheOptions
    {
        public string Prefix { get; set; } = "Aggregation_";

        public TimeSpan? ExpireTime { get; set; } = TimeSpan.FromHours(1);
    }
}
