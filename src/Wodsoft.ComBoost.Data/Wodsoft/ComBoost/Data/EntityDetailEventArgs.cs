using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity.Metadata;

namespace Wodsoft.ComBoost.Data
{
    public class EntityDetailEventArgs<T> : EventArgs
    {
        public EntityDetailEventArgs(T entity, IPropertyMetadata[] properties)
        {
            Entity = entity;
            Properties = properties;
        }

        public T Entity { get; private set; }

        public IPropertyMetadata[] Properties { get; set; }
    }
}
