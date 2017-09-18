using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Mock
{
    public class MockValueProvider : IConfigurableValueProvider
    {
        private Dictionary<string, object> _Values;
        private Dictionary<string, string> _Alias;

        public MockValueProvider()
        {
            _Values = new Dictionary<string, object>();
            _Alias = new Dictionary<string, string>();
        }

        public void SetValue(string key, object value)
        {
            if (value == null)
            {
                _Values.Remove(key);
                return;
            }
            if (_Values.ContainsKey(key))
                _Values[key] = value;
            else
                _Values.Add(key, value);
        }


        private ReadOnlyCollection<string> _Keys;
        public IReadOnlyCollection<string> Keys
        {
            get
            {
                if (_Keys == null)
                {
                    var keys = _Values.Keys.Concat(_Alias.Keys);
                    _Keys = new ReadOnlyCollection<string>(keys.Distinct().ToList());
                }
                return _Keys;
            }
        }

        public object GetValue(string name)
        {
            if (_Values.ContainsKey(name))
                return _Values[name];
            return null;
        }

        public object GetValue(string name, Type valueType)
        {
            object value = GetValue(name);
            if (value == null)
                return null;
            if (!valueType.IsAssignableFrom(value.GetType()))
            {
                var converter = TypeDescriptor.GetConverter(valueType);
                value = converter.ConvertFrom(value);
            }
            return value;
        }

        public bool ContainsKey(string name)
        {
            return _Values.ContainsKey(name);
        }

        public void SetAlias(string name, string aliasName)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (aliasName == null)
                throw new ArgumentNullException(nameof(aliasName));
            _Alias[aliasName] = name;
        }
    }
}
