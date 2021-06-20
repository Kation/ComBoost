using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity.Metadata;

namespace Wodsoft.ComBoost.Data
{
    public class EntityPropertyUpdateEventArgs<T> : DomainServiceEventArgs
    {
        public EntityPropertyUpdateEventArgs(T entity, IValueProvider valueProvider, IPropertyMetadata property, object value)
        {
            Entity = entity;
            ValueProvider = valueProvider;
            Property = property;
            Value = value;
        }

        public T Entity { get; private set; }

        public IValueProvider ValueProvider { get; private set; }
        
        public IPropertyMetadata Property { get; private set; }

        public object Value { get; private set; }
    }
}
