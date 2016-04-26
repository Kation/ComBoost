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
            Mode = AuthenticationRequiredMode.All;
        }

        /// <summary>
        /// Get or set the authentication required mode.
        /// </summary>
        public AuthenticationRequiredMode Mode { get; set; }

        /// <summary>
        /// Get or set is entity allow anonymous operate.
        /// </summary>
        public bool AllowAnonymous { get; set; }

        /// <summary>
        /// Get or set the roles to view entity.
        /// </summary>
        public object[] ViewRolesRequired { get; set; }

        /// <summary>
        /// Get or set the roles to add entity.
        /// </summary>
        public object[] AddRolesRequired { get; set; }

        /// <summary>
        /// Get or set the roles to edit entity.
        /// </summary>
        public object[] EditRolesRequired { get; set; }

        /// <summary>
        /// Get or set the roles to remove entity.
        /// </summary>
        public object[] RemoveRolesRequired { get; set; }

    }
}
