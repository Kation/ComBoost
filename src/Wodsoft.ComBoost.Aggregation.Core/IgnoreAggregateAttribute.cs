using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost.Aggregation
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class IgnoreAggregateAttribute : Attribute
    {
    }
}
