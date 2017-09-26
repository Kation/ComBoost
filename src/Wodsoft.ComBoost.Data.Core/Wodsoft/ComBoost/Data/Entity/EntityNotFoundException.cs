using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Data.Entity
{
    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException(Type entityType, object key)
        {
            if (entityType == null)
                throw new ArgumentNullException(nameof(entityType));
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            EntityType = entityType;
            Key = key;
        }

        public object Key { get; private set; }

        public Type EntityType { get; private set; }
    }
}
