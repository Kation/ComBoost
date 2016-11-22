using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.ComponentModel.DataAnnotations
{
    /// <summary>
    /// Password attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PasswordAttribute : Attribute
    {
        /// <summary>
        /// Initialize attribute.
        /// </summary>
        /// <param name="method">Method which setting password to property.</param>
        public PasswordAttribute(string method)
        {
            if (method == null)
                throw new ArgumentNullException("method");
            SetPasswordMethod = method;
        }

        /// <summary>
        /// Get the setting password method.
        /// </summary>
        public string SetPasswordMethod { get; private set; }
    }
}
