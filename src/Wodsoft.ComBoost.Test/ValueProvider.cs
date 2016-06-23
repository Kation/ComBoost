using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Test
{
    public class ValueProvider : Dictionary<string, object>, IValueProvider
    {
        public object GetValue(string name)
        {
            object value;
            TryGetValue(name, out value);
            return value;
        }

        public object GetValue(string name, Type valueType)
        {
            throw new NotImplementedException();
        }

        public T GetValue<T>(string name)
        {
            return (T)GetValue(name);
        }
    }
}
