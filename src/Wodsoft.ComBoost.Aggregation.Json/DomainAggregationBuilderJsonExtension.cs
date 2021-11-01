using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using System.Text.Json.Serialization;

namespace Wodsoft.ComBoost.Aggregation.Json
{
    public class DomainAggregationBuilderJsonExtension : IDomainAggregationsBuilderExtension
    {
        public IEnumerable<CustomAttributeBuilder> CreateIgnoredPropertyAttribute()
        {
            yield return new CustomAttributeBuilder(typeof(JsonIgnoreAttribute).GetConstructor(Array.Empty<Type>()), Array.Empty<object>());
        }
    }
}
