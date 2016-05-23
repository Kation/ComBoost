using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    public static class ValueProviderExtensions
    {
        public static object GetRequiredValue(this IValueProvider valueProvider, string name)
        {
            object value = valueProvider.GetValue(name);
            if (value == null)
                throw new ArgumentException("找不到所需值。");
            return value;
        }

        public static T GetRequriedValue<T>(this IValueProvider valueProvider, string name)
        {
            T value = valueProvider.GetValue<T>(name);
            if (value == null)
                throw new ArgumentException("找不到所需值。");
            return value;
        }
    }
}
