using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.ComponentModel.DataAnnotations
{
    /// <summary>
    /// Entity authentication attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class EntityAuthenticationAttribute : Attribute
    {
        /// <summary>
        /// Initialize attribute.
        /// </summary>
        public EntityAuthenticationAttribute()
        {
            AllowAnonymous = true;
            ViewRolesRequired = new string[0];
            AddRolesRequired = new string[0];
            EditRolesRequired = new string[0];
            RemoveRolesRequired = new string[0];
        }

        /// <summary>
        /// Get or set is entity allow anonymous operate.
        /// </summary>
        public bool AllowAnonymous { get; set; }

        /// <summary>
        /// Get or set the roles to view entity.
        /// </summary>
        public string[] ViewRolesRequired { get; set; }

        /// <summary>
        /// Get or set the roles to add entity.
        /// </summary>
        public string[] AddRolesRequired { get; set; }

        /// <summary>
        /// Get or set the roles to edit entity.
        /// </summary>
        public string[] EditRolesRequired { get; set; }

        /// <summary>
        /// Get or set the roles to remove entity.
        /// </summary>
        public string[] RemoveRolesRequired { get; set; }

    }
}
