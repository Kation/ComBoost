using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost.Data.Entity
{
    public abstract class EntityDTOBase<T> : EntityDTOBase, IEntityDTO<T>
    {
        public virtual T Id { get; set; }
    }
}
