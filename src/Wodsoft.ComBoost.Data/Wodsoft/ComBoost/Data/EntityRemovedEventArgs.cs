using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost.Data
{
    public class EntityRemovedEventArgs<T> : DomainServiceEventArgs
    {
        public EntityRemovedEventArgs(T entity)
        {
            Entity = entity;
        }

        public T Entity { get; }
    }
}
