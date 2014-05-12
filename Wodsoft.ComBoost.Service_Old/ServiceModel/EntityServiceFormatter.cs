using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Data.Entity;
using Wodsoft.Net.Service;

namespace System.ServiceModel
{
    public class EntityServiceFormatter : DataFormatter
    {
        public ICacheEntityContextBuilder ContextBuilder { get; private set; }

        public EntityServiceFormatter(ICacheEntityContextBuilder contextBuilder)
        {
            ContextBuilder = contextBuilder;
        }

        public override object Deserialize(Type objType, byte[] data)
        {
            if (data.Length == 16 && typeof(EntityBase).IsAssignableFrom(objType))
            {
                var queryable = ContextBuilder.GetContext(objType);
                return queryable.GetType().GetMethod("GetEntity").Invoke(queryable, new object[] { new Guid(data) });
            }
            return base.Deserialize(objType, data);
        }

        [ThreadStatic]
        private bool MainEntity;
        public override byte[] Serialize(object obj)
        {
            Type type = obj.GetType();
            if (!MainEntity && typeof(EntityBase).IsAssignableFrom(type))
            {
                return ((EntityBase)obj).Index.ToByteArray();
            }
            MainEntity = false;
            var data = base.Serialize(obj);
            MainEntity = true;
            return data;
        }
    }
}
