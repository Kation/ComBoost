using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    public class CacheEntry
    {
        public byte[] Value { get; set; }

        public string Key { get; set; }

        public DateTime ExpiredDate { get; set; }
    }
}
