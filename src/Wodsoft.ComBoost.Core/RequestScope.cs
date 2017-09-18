using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Wodsoft.ComBoost
{
    public class RequestScope
    {
        private static AsyncLocal<RequestScope> _Context = new AsyncLocal<RequestScope>();

        public static RequestScope Current { get { return _Context.Value; } set { _Context.Value = value; } }

        public RequestScope()
        {
            _Data = new Dictionary<string, object>();
        }

        private Dictionary<string, object> _Data;

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

        public T Get<T>(string key)
        {
            return (T)this[key];
        }
    }
}
