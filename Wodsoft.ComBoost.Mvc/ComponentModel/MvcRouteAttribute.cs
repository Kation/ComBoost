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
        /// <summary>
        /// Initialize mvc route attribute.
        /// </summary>
        /// <param name="area">Area name of entity controller.</param>
        public MvcRouteAttribute(string area)
        {
            Area = area;
        }

        /// <summary>
        /// Initialize mvc route attribute.
        /// </summary>
        /// <param name="area">Area name of entity controller.</param>
        /// <param name="controller">Controller name of entity controller.</param>
        public MvcRouteAttribute(string area, string controller)
            : this(area)
        {
            Controller = controller;
        }

        /// <summary>
        /// Initialize mvc route attribute.
        /// </summary>
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
