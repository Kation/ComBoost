using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.AspNetCore
{
    public abstract class HttpValueSelector
    {
        protected HttpValueSelector(HttpContext httpContext)
        {
            if (httpContext == null)
                throw new ArgumentNullException(nameof(httpContext));
            IgnoreCase = true;
            HttpContext = httpContext;
        }

        public HttpContext HttpContext { get; private set; }

        protected abstract string[] GetKeysCore();

        protected abstract object GetValueCore(string key);
        
        private string[] _Keys, _LowerKeys;

        public bool IgnoreCase { get; set; }

        public string[] GetKeys()
        {
            if (_Keys == null)
                _Keys = GetKeysCore();
            return _Keys;
        }

        public string[] GetLowerKeys()
        {
            if (_LowerKeys == null)
                _LowerKeys = GetKeys().Select(t => t.ToLower()).ToArray();
            return _LowerKeys;
        }

        public bool ContainsKey(string key)
        {
            var keys = IgnoreCase ? GetLowerKeys() : GetKeys();
            return keys.Contains(IgnoreCase ? key.ToLower() : key);
        }

        public object GetValue(string key)
        {
            return GetValueCore(key);
        }
    }
}
