using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.AspNetCore
{
    /// <summary>
    /// Http值选择器。
    /// </summary>
    public abstract class HttpValueSelector
    {
        /// <summary>
        /// 实例化Http值选择器。
        /// </summary>
        /// <param name="httpContext">Http上下文。</param>
        protected HttpValueSelector(HttpContext httpContext)
        {
            if (httpContext == null)
                throw new ArgumentNullException(nameof(httpContext));
            IgnoreCase = true;
            HttpContext = httpContext;
        }

        /// <summary>
        /// 获取Http上下文。
        /// </summary>
        public HttpContext HttpContext { get; private set; }

        /// <summary>
        /// 获取键。
        /// </summary>
        /// <returns>返回键名数组。</returns>
        protected abstract string[] GetKeysCore();

        /// <summary>
        /// 获取值。
        /// </summary>
        /// <param name="key">键名。</param>
        /// <returns></returns>
        protected abstract object GetValueCore(string key);
        
        private string[] _Keys, _LowerKeys;

        /// <summary>
        /// 获取或设置是否忽略大小写。
        /// </summary>
        public bool IgnoreCase { get; set; }

        /// <summary>
        /// 获取键名。
        /// </summary>
        /// <returns>返回键名数组。</returns>
        public string[] GetKeys()
        {
            if (_Keys == null)
                _Keys = GetKeysCore();
            return _Keys;
        }

        /// <summary>
        /// 获取小写键名。
        /// </summary>
        /// <returns>返回键名数组。</returns>
        public string[] GetLowerKeys()
        {
            if (_LowerKeys == null)
                _LowerKeys = GetKeys().Select(t => t.ToLower()).ToArray();
            return _LowerKeys;
        }

        /// <summary>
        /// 判断是否存在键名。
        /// </summary>
        /// <param name="key">键名。</param>
        /// <returns>如果存在返回true，否则返回false。</returns>
        public bool ContainsKey(string key)
        {
            var keys = IgnoreCase ? GetLowerKeys() : GetKeys();
            return keys.Contains(IgnoreCase ? key.ToLower() : key);
        }

        /// <summary>
        /// 获取值。
        /// </summary>
        /// <param name="key">键名。</param>
        /// <returns>返回值。</returns>
        public object GetValue(string key)
        {
            return GetValueCore(key);
        }
    }
}
