using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ComponentModel.DataAnnotations
{
    public class ValueFilterAttribute : Attribute
    {
        public ValueFilterAttribute(Type valueFilter, string dependencyProperty)
        {
            if (valueFilter == null)
                throw new ArgumentNullException("valueFilter");
            if (!typeof(ValueFilter).IsAssignableFrom(valueFilter))
                throw new ArgumentException("Type of \"" + valueFilter.Name + "\" is not inherit from ValueFilter.");
            ValueFilter = valueFilter;
            DependencyProperty = dependencyProperty;
        }

        public string DependencyProperty { get; private set; }

        public Type ValueFilter { get; private set; }
    }
}
