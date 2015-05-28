using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ComponentModel
{
    /// <summary>
    /// Pagination.
    /// </summary>
    public class Pagination : IPagination
    {
        /// <summary>
        /// Default number of items per page options.
        /// </summary>
        public static int[] DefaultPageSizeOption = new int[] { 10, 20, 30, 50 };

        /// <summary>
        /// Default number of items per page.
        /// </summary>
        public static int DefaultPageSize = 20;

        /// <summary>
        /// Initialize pagination.
        /// </summary>
        public Pagination()
        {
            PageSizeOption = DefaultPageSizeOption;
        }

        /// <summary>
        /// Initialize pagination.
        /// </summary>
        /// <param name="page">Page.</param>
        /// <param name="count">Number of total items.</param>
        /// <param name="size">Number of items per page.</param>
        public Pagination(int page, int count, int size)
            : this()
        {
            CurrentPage = page;
            CurrentSize = size;
            TotalPage = (int)Math.Ceiling(count / (double)size);
        }

        /// <summary>
        /// Initialize pagination.
        /// </summary>
        /// <param name="page">Page.</param>
        /// <param name="count">Number of total items.</param>
        public Pagination(int page, int count)
            : this(page, count, DefaultPageSize) { }

        /// <summary>
        /// Get or set the page size options.
        /// </summary>
        public int[] PageSizeOption { get; set; }

        /// <summary>
        /// Get or set the total page.
        /// </summary>
        public int TotalPage { get; set; }

        /// <summary>
        /// Get or set the current page.
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// Get or set the current number of items per page.
        /// </summary>
        public int CurrentSize { get; set; }
    }
}
