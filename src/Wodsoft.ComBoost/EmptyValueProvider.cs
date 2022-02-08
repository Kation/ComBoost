using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Wodsoft.ComBoost
{
    public class EmptyValueProvider : IConfigurableValueProvider
    {
        private Dictionary<string, object> _Values;
        private Dictionary<string, string> _Alias;

        public EmptyValueProvider()
        {
            _Values = new Dictionary<string, object>();
            _Alias = new Dictionary<string, string>();
        }

        public virtual void SetValue(string name, object value)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (value == null)
            {
                _Values.Remove(name);
                return;
            }
            if (_Values.ContainsKey(name))
                _Values[name] = value;
            else
                _Values.Add(name, value);
            _Keys = null;
        }

        private ValueKeyCollection? _Keys;
        public virtual IValueKeyCollection Keys
        {
            get
            {
                if (_Keys == null)
                {
                    var keys = _Values.Keys.Concat(_Alias.Keys);
                    _Keys = new ValueKeyCollection(keys.Distinct().ToList());
                }
                return _Keys;
            }
        }

        protected virtual object? GetValue(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (_Values.ContainsKey(name))
                return _Values[name];
            return null;
        }

        public virtual object? GetValue(string name, Type valueType)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (valueType == null)
                throw new ArgumentNullException(nameof(valueType));
            
            object? value = GetValue(name);
            if (value == null)
            {
                if (!_Alias.TryGetValue(name, out string aliasName))
                    return null;
                name = aliasName;
                value = GetValue(name);
            }
            if (value == null)
                return null;
            var currentType = value.GetType();
            if (!valueType.IsAssignableFrom(currentType))
            {
                var converter = TypeDescriptor.GetConverter(valueType);
                if (converter.CanConvertFrom(currentType))
                    value = converter.ConvertFrom(value);
                else
                {
                    var currentConverter = TypeDescriptor.GetConverter(currentType);
                    var currentStringValue = currentConverter.ConvertToString(value);
                    value = converter.ConvertFromString(currentStringValue);
                }
            }
            return value;
        }

        public virtual bool ContainsKey(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            return Keys.ContainsKey(name);
        }

        public virtual void SetAlias(string name, string aliasName)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (aliasName == null)
                throw new ArgumentNullException(nameof(aliasName));
            _Alias[aliasName] = name;
            _Keys = null;
        }
    }
}
