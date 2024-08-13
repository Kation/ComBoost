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
        /// <param name="name">名称。</param>
        /// <param name="valueType">值类型。</param>
        /// <returns>返回值。</returns>
        object? GetValue(string name, Type valueType);

        /// <summary>
        /// 获取存在的键集合。
        /// </summary>
        IValueKeyCollection Keys { get; }

        /// <summary>
        /// 返回值提供器是否包含名称。
        /// </summary>
        /// <param name="name">名称。</param>
        /// <returns></returns>
        bool ContainsKey(string name);

        /// <summary>
        /// 设置值。
        /// </summary>
        /// <param name="name">名称。</param>
        /// <param name="value">值。</param>
        void SetValue(string name, object? value);

        /// <summary>
        /// 设置别名。
        /// </summary>
        /// <param name="name">名称。</param>
        /// <param name="aliasName">别名。</param>
        void SetAlias(string name, string aliasName);
    }
}
