using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    public interface IValueProvider
    {
        object GetValue(Type valueType);

        object GetValue(string name);

        object GetValue(string name, Type valueType);

        ICollection<string> Keys { get; }
    }
}
