using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true, Inherited = true)]
    public class DomainDistributedEventFeatureAttribute : Attribute
    {
        public DomainDistributedEventFeatureAttribute(string feature)
        {
            if (feature == null)
                throw new ArgumentNullException(nameof(feature));
            Feature = feature.ToUpper();
        }

        /// <summary>
        /// Get the feature normalized(upper) name.
        /// </summary>
        public string Feature { get; }
    }
}
