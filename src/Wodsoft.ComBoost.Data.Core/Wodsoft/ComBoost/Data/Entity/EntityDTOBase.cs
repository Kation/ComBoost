using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost.Data.Entity
{
    public abstract class EntityDTOBase : IEntityDTO
    {
        public virtual DateTimeOffset CreationDate { get; set; }

        public virtual DateTimeOffset ModificationDate { get; set; }
    }
}
