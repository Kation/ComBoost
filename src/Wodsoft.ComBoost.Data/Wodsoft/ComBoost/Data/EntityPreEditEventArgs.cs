using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost.Data
{
    public class EntityPreEditEventArgs<T> : DomainServiceEventArgs
    {
        public EntityPreEditEventArgs(T entity)
        {
            Entity = entity;
        }

        public T Entity { get; }
    }
}
