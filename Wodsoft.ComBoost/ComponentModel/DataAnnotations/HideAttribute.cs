using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.ComponentModel.DataAnnotations
{
    /// <summary>
    /// Property hiding attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class HideAttribute : Attribute
    {
        /// <summary>
        /// Initialize attribute.
        /// </summary>
        public HideAttribute() 
        {
            IsHiddenOnEdit = true;
            IsHiddenOnView = true;
        }

        /// <summary>
        /// Get or set is property hidden in viewlist.
        /// </summary>
        public bool IsHiddenOnView { get; set; }

        /// <summary>
        /// Get or set is property hidden while editing.
        /// </summary>
        public bool IsHiddenOnEdit { get; set; }
    }
}