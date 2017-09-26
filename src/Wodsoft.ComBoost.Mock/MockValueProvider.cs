using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Mock
{
    public class MockValueProvider : EmptyValueProvider
    {
        private Dictionary<string, object> _Values;

        public MockValueProvider()
        {
            _Values = new Dictionary<string, object>();
            IgnoreCase = false;
        }

        public bool IgnoreCase { get; set; }

        private ReadOnlyCollection<string> _Keys;
        public override IReadOnlyCollection<string> Keys
        {
            get
            {
                if (_Keys == null)
                {
                    IEnumerable<string> keys = base.Keys;
                    if (IgnoreCase)
                        keys = keys.Select(t => t.ToLower());
                    _Keys = new ReadOnlyCollection<string>(keys.Distinct().ToList());
                }
                return _Keys;
            }
        }

        protected override object GetValue(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (IgnoreCase)
                name = name.ToLower();
            return base.GetValue(name);
        }

        public override void SetValue(string name, object value)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (IgnoreCase)
                name = name.ToLower();
            base.SetValue(name, value);
        }

        public override void SetAlias(string name, string aliasName)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (aliasName == null)
                throw new ArgumentNullException(nameof(aliasName));
            if (IgnoreCase)
            {
                name = name.ToLower();
                aliasName = aliasName.ToLower();
            }
            base.SetAlias(name, aliasName);
        }
    }
}
