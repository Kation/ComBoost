using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace Wodsoft.ComBoost.Aggregation.NewtonsoftJson
{
    public class DomainAggregationBuilderJsonExtension : IDomainAggregationsBuilderExtension
    {
        public IEnumerable<CustomAttributeBuilder> CreateIgnoredPropertyAttribute()
        {
            yield return new CustomAttributeBuilder(typeof(JsonIgnoreAttribute).GetConstructor(Array.Empty<Type>()), Array.Empty<object>());
        }
    }
}
