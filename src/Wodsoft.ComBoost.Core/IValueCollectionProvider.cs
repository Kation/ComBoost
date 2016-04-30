using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    public interface IValueCollectionProvider
    {
        IValueProvider GetValueProvider(string name);
    }
}
