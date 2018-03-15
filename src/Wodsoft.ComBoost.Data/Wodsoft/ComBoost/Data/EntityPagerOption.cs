using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Data
{
    public class EntityPagerOption
    {
        public int CurrentPage { get; set; }

        public int CurrentSize { get; set; }

        public int DefaultSize { get; set; }

        public int[] PageSizeOption { get; set; }
    }
}
