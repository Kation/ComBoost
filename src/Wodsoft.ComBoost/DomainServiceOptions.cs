using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    public class DomainServiceOptions : IDomainServiceOptions
    {
        private Dictionary<Type, object> _Local;

        public DomainServiceOptions()
        {
            _Local = new Dictionary<Type, object>();
        }

        public virtual void SetOption(Type optionType, object option)
        {
            if (_Local.ContainsKey(optionType))
                _Local[optionType] = option;
            else
                _Local.Add(optionType, option);
        }

        public virtual object GetOption(Type optionType)
        {
            object option;
            _Local.TryGetValue(optionType, out option);
            return option;
        }

        public virtual void RemoveOption(Type optionType)
        {
            _Local.Remove(optionType);
        }
    }
}
