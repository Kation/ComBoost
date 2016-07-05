using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    public interface ICacheEntry
    {
        object Value { get; set; }

        string Key { get; set; }

        DateTime ExpiredDate { get; set; }

        byte[] GetData();
    }
}
