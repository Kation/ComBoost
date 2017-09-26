using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    /// <summary>
    /// 值提供器扩展。
    /// </summary>
    public static class ValueProviderExtensions
    {
        /// <summary>
        /// 获取值。
        /// </summary>
        /// <typeparam name="T">值类型。</typeparam>
        /// <param name="provider">值提供器。</param>
        /// <param name="name">名称。</param>
        /// <returns>返回值。</returns>
        public static T GetValue<T>(this IValueProvider provider, string name)
        {
            object value = provider.GetValue(name, typeof(T));
            if (value == null)
                return default(T);
            return (T)value;
        }
        
        /// <summary>
        /// 获取必须存在的值。
        /// </summary>
        /// <param name="valueProvider">值提供器。</param>
        /// <param name="name">名称。</param>
        /// <param name="valueType">值类型。</param>
        /// <returns>返回值。</returns>
        public static object GetRequiredValue(this IValueProvider valueProvider, string name, Type valueType)
        {
            object value = valueProvider.GetValue(name, valueType);
            if (value == null)
                throw new ArgumentException("找不到所需值。", name);
            return value;
        }

        /// <summary>
        /// 获取必须存在的值。
        /// </summary>
        /// <typeparam name="T">值类型。</typeparam>
        /// <param name="valueProvider">值提供器。</param>
        /// <param name="name">名称。</param>
        /// <returns>返回值。</returns>
        public static T GetRequiredValue<T>(this IValueProvider valueProvider, string name)
        {
            object value = valueProvider.GetValue(name, typeof(T));
            if (value == null)
                throw new ArgumentException("找不到所需值。", name);
            return (T)value;
        }        
    }
}
