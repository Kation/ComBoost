using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Wodsoft.ComBoost.Data.Entity
{
    public abstract class EntityDTOBase<T> : EntityDTOBase, IEntityDTO<T>
    {
        [AllowNull]
        public virtual T Id { get; set; }
    }
}
