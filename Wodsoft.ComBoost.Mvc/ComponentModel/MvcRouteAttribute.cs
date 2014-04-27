using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ComponentModel
{
    /// <summary>
    /// Mvc route for entity attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class MvcRouteAttribute : Attribute
    {
        public MvcRouteAttribute(string area)
        {
            Area = area;
        }

        public MvcRouteAttribute(string area, string controller)
            : this(area)
        {
            Controller = controller;
        }

        public MvcRouteAttribute()
        {

        }

        /// <summary>
        /// Get or set the area name of mvc.
        /// </summary>
        public string Area { get; set; }

        /// <summary>
        /// Get or set the controller name of mvc.
        /// </summary>
        public string Controller { get; set; }
    }
}
