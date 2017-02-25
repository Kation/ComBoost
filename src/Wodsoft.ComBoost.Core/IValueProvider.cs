using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    /// <summary>
    /// 值提供器。
    /// </summary>
    public interface IValueProvider
    {
        /// <summary>
        /// 获取值。
        /// </summary>
        /// <param name="valueType">值类型。</param>
        /// <returns>返回值。</returns>
        object GetValue(Type valueType);

        /// <summary>
        /// 获取值。
        /// </summary>
        /// <param name="name">名称。</param>
        /// <returns>返回值。</returns>
        object GetValue(string name);

        /// <summary>
        /// 获取值。
        /// </summary>
        /// <param name="name">名称。</param>
        /// <param name="valueType">值类型。</param>
        /// <returns>返回值。</returns>
        object GetValue(string name, Type valueType);

        /// <summary>
        /// 获取存在的键集合。
        /// </summary>
        ICollection<string> Keys { get; }
    }
}
