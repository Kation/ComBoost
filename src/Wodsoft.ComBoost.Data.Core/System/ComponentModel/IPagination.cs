using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ComponentModel
{
    /// <summary>
    /// Pagination interface.
    /// </summary>
    public interface IPagination
    {
        /// <summary>
        /// Get the items per page options.
        /// </summary>
        int[] PageSizeOption { get; }

        /// <summary>
        /// Get the total page.
        /// </summary>
        int TotalPage { get; }

        /// <summary>
        /// Get the current page.
        /// </summary>
        int CurrentPage { get; }

        /// <summary>
        /// Get the items per page.
        /// </summary>
        int CurrentSize { get; }
    }
}
