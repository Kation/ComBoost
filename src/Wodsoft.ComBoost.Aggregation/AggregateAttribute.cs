using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost.Aggregation
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class AggregateAttribute : Attribute
    {
        public AggregateAttribute(Type type, string name)
        {
            AggregationType = type ?? throw new ArgumentNullException(nameof(type));
            AggregationName = name ?? throw new ArgumentNullException(nameof(name));
        }
        
        public Type AggregationType { get; }

        public string AggregationName { get; }

        public bool IsSelfIgnored { get; set; }

        public bool IsExposed { get; set; }
    }
}
