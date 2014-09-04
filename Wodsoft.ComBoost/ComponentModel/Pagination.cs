using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ComponentModel
{
    public class Pagination : IPagination
    {
        public Pagination()
        {
            PageSizeOption = EntityViewModel.DefaultPageSizeOption;
        }

        public Pagination(int page, int count, int size)
            : this()
        {
            CurrentPage = page;
            CurrentSize = size;
            TotalPage = (int)Math.Ceiling(count / (double)size);
        }

        public Pagination(int page, int count)
            : this(page, count, EntityViewModel.DefaultPageSize) { }

        public int[] PageSizeOption { get; set; }

        public int TotalPage { get; set; }

        public int CurrentPage { get; set; }

        public int CurrentSize { get; set; }
    }
}
