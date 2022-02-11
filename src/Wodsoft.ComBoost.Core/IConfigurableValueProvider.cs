using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost
{
    /// <summary>
    /// 可配置的值提供器。
    /// </summary>
    public interface IConfigurableValueProvider : IValueProvider
    {
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
