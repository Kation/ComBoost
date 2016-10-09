using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    public static class ValueProviderExtensions
    {
        public static T GetValue<T>(this IValueProvider provider, string name)
        {
            object value = provider.GetValue(name, typeof(T));
            if (value == null)
                return default(T);
            return (T)value;
        }

        public static T GetValue<T>(this IValueProvider provider)
        {
            object value = provider.GetValue(typeof(T));
            if (value == null)
                return default(T);
            return (T)value;
        }

        public static object GetRequiredValue(this IValueProvider valueProvider, string name)
        {
            object value = valueProvider.GetValue(name);
            if (value == null)
                throw new ArgumentException("找不到所需值。");
            return value;
        }

        public static object GetRequiredValue(this IValueProvider valueProvider, string name, Type valueType)
        {
            object value = valueProvider.GetValue(name, valueType);
            if (value == null)
                throw new ArgumentException("找不到所需值。");
            return value;
        }

        public static T GetRequiredValue<T>(this IValueProvider valueProvider, string name)
        {
            T value = valueProvider.GetValue<T>(name);
            if (value == null)
                throw new ArgumentException("找不到所需值。");
            return value;
        }

        public static T GetRequiredValue<T>(this IValueProvider valueProvider)
        {
            T value = (T)valueProvider.GetValue(typeof(T));
            if (value == null)
                throw new ArgumentException("找不到所需值。");
            return value;

        }
    }
}
