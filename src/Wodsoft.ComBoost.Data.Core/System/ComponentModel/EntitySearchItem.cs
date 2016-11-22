using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ComponentModel
{
    /// <summary>
    /// Entity search item.
    /// </summary>
    public class EntitySearchItem
    {
        /// <summary>
        /// Get or set the property to search.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Get or set the morethan value.
        /// </summary>
        public double? Morethan { get; set; }

        /// <summary>
        /// Get or set the lessthan value.
        /// </summary>
        public double? Lessthan { get; set; }

        /// <summary>
        /// Get or set the contains value.
        /// </summary>
        public string Contains { get; set; }

        /// <summary>
        /// Get or set the morethan date value.
        /// </summary>
        public DateTime? MorethanDate { get; set; }

        /// <summary>
        /// Get or set the lessthan date value.
        /// </summary>
        public DateTime? LessthanDate { get; set; }

        /// <summary>
        /// Get or set the equal value.
        /// </summary>
        public bool? Equal { get; set; }

        /// <summary>
        /// Get or set the enum value.
        /// </summary>
        public string Enum { get; set; }
    }
}
