using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Mock
{
    public class MockValueProvider : IValueProvider
    {
        private Dictionary<string, object> _Values;
        private Dictionary<Type, object> _TypeValues;

        public MockValueProvider()
        {
            _Values = new Dictionary<string, object>();
            _TypeValues = new Dictionary<Type, object>();
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

        public void SetValue<T>(T value)
        {
            Type type = typeof(T);
            if (value == null)
            {
                _TypeValues.Remove(type);
                return;
            }
            if (_TypeValues.ContainsKey(type))
                _TypeValues[type] = value;
            else
                _TypeValues.Add(type, value);
        }

        public ICollection<string> Keys { get { return _Values.Keys; } }

        public object GetValue(string name)
        {
            if (_Values.ContainsKey(name))
                return _Values[name];
            return null;
        }

        public object GetValue(Type valueType)
        {
            if (_TypeValues.ContainsKey(valueType))
                return _TypeValues[valueType];
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
    }
}
