using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    public class DomainContextDataBag : System.Dynamic.DynamicObject
    {
        private Dictionary<string, object> _Data;

        public DomainContextDataBag()
        {
            _Data = new Dictionary<string, object>();
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            _Data.TryGetValue(binder.Name, out result);
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            if (_Data.ContainsKey(binder.Name))
                _Data[binder.Name] = value;
            else
                _Data.Add(binder.Name, value);
            return true;
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            if (indexes.Length > 1)
                throw new NotSupportedException();
            string key = indexes[0] as string;
            if (key == null)
                throw new NotSupportedException();
            _Data.TryGetValue(key, out result);
            return true;
        }

        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            if (indexes.Length > 1)
                throw new NotSupportedException();
            string key = indexes[0] as string;
            if (key == null)
                throw new NotSupportedException();
            if (_Data.ContainsKey(key))
                _Data[key] = value;
            else
                _Data.Add(key, value);
            return true;
        }
    }
}
