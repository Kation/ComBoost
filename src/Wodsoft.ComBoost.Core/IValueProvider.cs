using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    public interface IValueProvider
    {
        object GetValue(string name);
        T GetValue<T>(string name);
    }
}
