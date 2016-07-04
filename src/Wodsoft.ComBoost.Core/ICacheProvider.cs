using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    public interface ICacheProvider
    {
        ICache GetCache();

        ICache GetCache(string location);
    }
}
