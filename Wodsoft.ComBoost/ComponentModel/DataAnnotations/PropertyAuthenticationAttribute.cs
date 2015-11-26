using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.ComponentModel.DataAnnotations
{
    /// <summary>
    /// Property authentication attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PropertyAuthenticationAttribute : Attribute
    {
        /// <summary>
        /// Initialize attribute.
        /// </summary>
        public PropertyAuthenticationAttribute()
        {
            AllowAnonymous = true;
            EditRolesRequired = new string[0];
            ViewRolesRequired = new string[0];
        }

        /// <summary>
        /// Get or set the property allow anonymous view.
        /// </summary>
        public bool AllowAnonymous { get; set; }

        /// <summary>
        /// Get or set the roles to edit.
        /// </summary>
        public string[] EditRolesRequired { get; set; }

        /// <summary>
        /// Get or set the roles to view.
        /// </summary>
        public string[] ViewRolesRequired { get; set; }
    }
}
