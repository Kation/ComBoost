using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Wodsoft.ComBoost.Test
{
    public class ClientViewModel<T> : IPagination
    {
        public int[] PageSizeOption { get; set; }

        public int TotalPage { get; set; }

        public int CurrentPage { get; set; }

        public int CurrentSize { get; set; }

        public int TotalCount { get; set; }

        public T[] Items { get; set; }
    }
}
