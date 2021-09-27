using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost.Data
{
    public class EntityPreCreateEventArgs<T> : DomainServiceEventArgs
    {
        public EntityPreCreateEventArgs(T entity)
        {
            Entity = entity;
        }

        public T Entity { get; }
    }
}
