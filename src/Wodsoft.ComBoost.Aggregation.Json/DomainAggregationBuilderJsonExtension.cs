using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Text.Json.Serialization;

namespace Wodsoft.ComBoost.Aggregation.Json
{
    public class DomainAggregationBuilderJsonExtension : IDomainAggregationsBuilderExtension
    {
        private static readonly ConstructorInfo _JsonIgnoreAttributeConstructor = typeof(JsonIgnoreAttribute).GetConstructor(Array.Empty<Type>())!;

        public IEnumerable<CustomAttributeBuilder> CreateIgnoredPropertyAttribute()
        {
            yield return new CustomAttributeBuilder(_JsonIgnoreAttributeConstructor, Array.Empty<object>());
        }
    }
}
