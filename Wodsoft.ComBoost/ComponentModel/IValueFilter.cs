using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ComponentModel
{
    /// <summary>
    /// Value filter interface.
    /// </summary>
    public interface IValueFilter
    {
        /// <summary>
        /// Get values.
        /// </summary>
        /// <param name="dependencyProperty">Dependency property name.</param>
        /// <param name="dependencyValue">Dependency property value.</param>
        /// <returns></returns>
        NameValueCollection GetValues(string dependencyProperty, string dependencyValue);
    }
}
