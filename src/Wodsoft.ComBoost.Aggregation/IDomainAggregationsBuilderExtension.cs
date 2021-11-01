using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace Wodsoft.ComBoost.Aggregation
{
    public interface IDomainAggregationsBuilderExtension
    {
        IEnumerable<CustomAttributeBuilder> CreateIgnoredPropertyAttribute();
    }
}
