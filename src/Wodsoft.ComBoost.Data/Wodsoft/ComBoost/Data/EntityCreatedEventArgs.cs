using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost.Data
{
    public class EntityCreatedEventArgs<T> : DomainServiceEventArgs
    {
        public EntityCreatedEventArgs(T entity)
        {
            Entity = entity;
        }

        public T Entity { get; }
    }
}
