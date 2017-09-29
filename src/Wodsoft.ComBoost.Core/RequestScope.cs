using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Wodsoft.ComBoost
{
    /// <summary>
    /// 请求上下文。
    /// </summary>
    public class RequestScope
    {
        private static AsyncLocal<RequestScope> _Context = new AsyncLocal<RequestScope>();

        /// <summary>
        /// 获取当前请求上下文。
        /// </summary>
        public static RequestScope Current { get { return _Context.Value; } set { _Context.Value = value; } }

        /// <summary>
        /// 实例化请求上下文。
        /// </summary>
        public RequestScope()
        {
            _Data = new Dictionary<string, object>();
        }

        private Dictionary<string, object> _Data;

        /// <summary>
        /// 获取或设置对象。
        /// </summary>
        /// <param name="key">键名称。</param>
        /// <returns></returns>
        public object this[string key]
        {
            get
            {
                if (_Data.TryGetValue(key, out object value))
                    return value;
                return null;
            }
            set
            {
                _Data[key] = value;
            }
        }

        /// <summary>
        /// 获取对象。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="key">键名称。</param>
        /// <returns></returns>
        public T Get<T>(string key)
        {
            return (T)this[key];
        }
    }
}
