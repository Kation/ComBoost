using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Data
{
    public class EntityRemoveEventArgs<T> : EventArgs
    {
        public EntityRemoveEventArgs(T entity)
        {
            Entity = entity;
        }

        public T Entity { get; private set; }

        public bool IsCanceled { get; set; }
    }
}
