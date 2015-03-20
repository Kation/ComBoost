using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ComponentModel.DataAnnotations
{
    /// <summary>
    /// Property using value filter attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ValueFilterAttribute : Attribute
    {
        /// <summary>
        /// Initialize attribute.
        /// </summary>
        /// <param name="valueFilter">Type of value filter.</param>
        /// <param name="dependencyProperty">The dependency property name.</param>
        public ValueFilterAttribute(Type valueFilter, string dependencyProperty)
        {
            if (valueFilter == null)
                throw new ArgumentNullException("valueFilter");
            if (dependencyProperty == null)
                throw new ArgumentNullException("dependencyProperty");
            if (!typeof(IValueFilter).IsAssignableFrom(valueFilter))
                throw new ArgumentException("Type of \"" + valueFilter.Name + "\" is not implement IValueFilter.");
            ValueFilter = valueFilter;
            DependencyProperty = dependencyProperty;
        }

        /// <summary>
        /// Get the dependency property name.
        /// </summary>
        public string DependencyProperty { get; private set; }

        /// <summary>
        /// Get the type of value filter for property.
        /// </summary>
        public Type ValueFilter { get; private set; }
    }
}
