using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Data.Entity.Metadata
{
    public class EntityMetadataDataBag : DynamicObject
    {
        private ConcurrentDictionary<string, object> _Values;

        public EntityMetadataDataBag()
        {
            _Values = new ConcurrentDictionary<string, object>();
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            _Values.TryGetValue(binder.Name, out result);
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            _Values.AddOrUpdate(binder.Name, value, (k, v) =>
            {
                return value;
            });
            return true;
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            if (indexes.Length > 1)
                throw new NotSupportedException();
            string key = indexes[0] as string;
            if (string.IsNullOrEmpty(key))
                throw new NotSupportedException();
            _Values.TryGetValue(key, out result);
            return true;
        }

        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            if (indexes.Length > 1)
                throw new NotSupportedException();
            string key = indexes[0] as string;
            if (string.IsNullOrEmpty(key))
                throw new NotSupportedException();
            _Values.AddOrUpdate(key, value, (k, v) =>
            {
                return value;
            });
            return true;
        }
    }
}
