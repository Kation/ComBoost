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

        private ReadOnlyCollection<string> _Keys;
        public virtual IReadOnlyCollection<string> Keys
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

        public virtual object GetValue(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (_Values.ContainsKey(name))
                return _Values[name];
            return null;
        }

        public virtual object GetValue(string name, Type valueType)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (valueType == null)
                throw new ArgumentNullException(nameof(valueType));
            if (_Alias.TryGetValue(name, out string aliasName))
                name = aliasName;
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

        public virtual bool ContainsKey(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            return Keys.Contains(name);
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
