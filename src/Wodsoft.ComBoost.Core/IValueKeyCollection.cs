using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost
{
    /// <summary>
    /// 值的键集合。
    /// </summary>
    public interface IValueKeyCollection : IReadOnlyCollection<string>
    {
        /// <summary>
        /// 是否包含键。
        /// </summary>
        /// <param name="key">键。</param>
        /// <returns></returns>
        bool ContainsKey(string key);

        /// <summary>
        /// 是否包含前缀。
        /// </summary>
        /// <param name="prefix">前缀。</param>
        /// <param name="separators">分隔符。</param>
        /// <returns></returns>
        bool ContainsPrefix(string prefix, params char[] separators);

        /// <summary>
        /// 根据前缀获取键。
        /// </summary>
        /// <param name="prefix">前缀。</param>
        /// <param name="separators">分隔符。</param>
        /// <returns></returns>
        IDictionary<string, string> GetKeysFromPrefix(string prefix, params char[] separators);
    }
}
