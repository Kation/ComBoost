using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity.Metadata;

namespace Wodsoft.ComBoost.Data
{
    public class EntityUpdateEventArgs<T> : DomainServiceEventArgs
    {
        public EntityUpdateEventArgs(T entity, IValueProvider valueProvider, IPropertyMetadata[] properties)
        {
            Entity = entity;
            ValueProvider = valueProvider;
            Properties = properties;
        }

        public T Entity { get; private set; }

        public IValueProvider ValueProvider { get; private set; }

        public IPropertyMetadata[] Properties { get; set; }
    }
}
