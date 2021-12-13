using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost.Aggregation
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class AggregateAttribute : Attribute
    {
        public AggregateAttribute(Type type, string name, params string[] keyProperties)
        {
            AggregationType = type ?? throw new ArgumentNullException(nameof(type));
            AggregationName = name ?? throw new ArgumentNullException(nameof(name));
            if (keyProperties == null)
                throw new ArgumentNullException(nameof(keyProperties));
            if (keyProperties.Length == 0)
                throw new ArgumentException("Must have a property.");
            KeyProperties = keyProperties;
        }
        
        public Type AggregationType { get; }

        public string AggregationName { get; }

        public string[] KeyProperties { get; }

        public bool IsSelfIgnored { get; set; }

        public bool IsExposed { get; set; }

        public bool IsNestAggregate { get; set; } = true;
    }
}
