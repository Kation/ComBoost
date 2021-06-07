using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Wodsoft.ComBoost
{
    public class DomainRpcValueProvider : IConfigurableValueProvider
    {
        private IDomainRpcRequest _request;
        private Dictionary<string, object> _values = new Dictionary<string, object>();
        private Dictionary<string, string> _alias = new Dictionary<string, string>();

        public DomainRpcValueProvider(IDomainRpcRequest request)
        {
            _request = request ?? throw new ArgumentNullException(nameof(request));
            Keys = new ValueKeyCollection(_request.Values.Keys);
        }

        public IValueKeyCollection Keys { get; }

        public bool ContainsKey(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            return _values.ContainsKey(name) || _request.Values.ContainsKey(name);
        }

        public object GetValue(string name, Type valueType)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (valueType == null)
                throw new ArgumentNullException(nameof(valueType));
            if (_values.TryGetValue(name, out var sourceValue) || (_alias.ContainsKey(name) && _values.TryGetValue(name, out sourceValue)))
            {
                if (sourceValue != null && !valueType.IsAssignableFrom(sourceValue.GetType()))
                    return null;
                return sourceValue;
            }
            if (_request.Values.TryGetValue(name, out var value) || (_alias.ContainsKey(name) && _request.Values.TryGetValue(name, out value)))
            {
                if (valueType == typeof(string))
                    return value;
                var converter = TypeDescriptor.GetConverter(valueType);
                if (converter == null)
                    throw new InvalidCastException("值不能转换为目标类型“" + valueType.Name + "”。");
                if (!converter.CanConvertFrom(typeof(string)))
                    throw new InvalidCastException("值不能转换为目标类型“" + valueType.Name + "”。");
                return converter.ConvertFromString(value);
            }
            return null;
        }

        public void SetAlias(string name, string aliasName)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (aliasName == null)
                throw new ArgumentNullException(nameof(aliasName));
            _alias[aliasName] = name;
        }

        public void SetValue(string name, object value)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (value == null)
            {
                _values.Remove(name);
                return;
            }
            if (_values.ContainsKey(name))
                _values[name] = value;
            else
                _values.Add(name, value);
        }
    }
}
