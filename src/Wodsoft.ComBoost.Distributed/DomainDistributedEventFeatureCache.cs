using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Wodsoft.ComBoost
{
    internal class DomainDistributedEventFeatureCache<T>
    {
        static DomainDistributedEventFeatureCache()
        {
            List<DomainDistributedEventFeatureAttribute> features = new List<DomainDistributedEventFeatureAttribute>();
            features.AddRange(typeof(T).GetCustomAttributes<DomainDistributedEventFeatureAttribute>(true));
            features.AddRange(typeof(T).GetInterfaces().SelectMany(t => t.GetCustomAttributes<DomainDistributedEventFeatureAttribute>()));
            Features = features.Select(t => t.Feature).Distinct().ToList().AsReadOnly();
        }

        public static IReadOnlyList<string> Features { get; }
    }
}
