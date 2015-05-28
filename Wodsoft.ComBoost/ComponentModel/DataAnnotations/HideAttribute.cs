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
            IsHiddenOnCreate = true;
        }

        /// <summary>
        /// Get or set is property hidden in viewlist.
        /// Default is true.
        /// </summary>
        public bool IsHiddenOnView { get; set; }

        /// <summary>
        /// Get or set is property hidden while editing.
        /// Default is true.
        /// </summary>
        public bool IsHiddenOnEdit { get; set; }

        /// <summary>
        /// Get or set is property hidden while creating.
        /// Default is true.
        /// </summary>
        public bool IsHiddenOnCreate { get; set; }

        private bool? _IsHiddenOnDetail;
        /// <summary>
        /// Get or set is property hidden while editing.
        /// Default is IsHiddenOnView &amp;&amp; IsHiddenOnEdit.
        /// </summary>
        public bool IsHiddenOnDetail
        {
            get
            {
                if (_IsHiddenOnDetail.HasValue)
                    return _IsHiddenOnDetail.Value;
                return IsHiddenOnView && IsHiddenOnEdit;
            }
            set
            {
                _IsHiddenOnDetail = value;
            }
        }
    }
}