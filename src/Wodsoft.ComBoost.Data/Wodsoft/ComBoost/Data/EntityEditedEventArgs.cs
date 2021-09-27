using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost.Data
{
    public class EntityEditedEventArgs<T> : DomainServiceEventArgs
    {
        public EntityEditedEventArgs(T entity)
        {
            Entity = entity;
        }

        public T Entity { get; }
    }
}
