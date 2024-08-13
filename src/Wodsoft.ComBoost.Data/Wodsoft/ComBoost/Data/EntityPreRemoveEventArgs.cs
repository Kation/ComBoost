using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost.Data
{
    public class EntityPreRemoveEventArgs<T> : DomainServiceEventArgs
    {
        public EntityPreRemoveEventArgs(T entity)
        {
            Entity = entity;
        }

        public T Entity { get; }
    }
}
