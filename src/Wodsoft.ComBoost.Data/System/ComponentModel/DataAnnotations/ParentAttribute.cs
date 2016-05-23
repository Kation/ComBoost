using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.ComponentModel.DataAnnotations
{
    /// <summary>
    /// Entity parent attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ParentAttribute : Attribute
    {
        /// <summary>
        /// Initialize attribute.
        /// </summary>
        /// <param name="parent">Type of parent.</param>
        /// <param name="propertyName">Property of parent.</param>
        public ParentAttribute(Type parent, string propertyName)
        {
            if (parent == null)
                throw new ArgumentNullException(nameof(parent));
            if (propertyName == null)
                throw new ArgumentNullException(nameof(propertyName));
            //if (!typeof(IEntity).IsAssignableFrom(parent))
            //    throw new NotSupportedException("Type of parent must inherit IEntity.");
            Parent = parent;
            PropertyName = propertyName;
        }

        /// <summary>
        /// Get the type of parent.
        /// </summary>
        public Type Parent { get; private set; }

        /// <summary>
        /// Get the property of parent.
        /// </summary>
        public string PropertyName { get; private set; }
    }
}
