using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Wodsoft.ComBoost.Aggregation.NewtonsoftJson
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
