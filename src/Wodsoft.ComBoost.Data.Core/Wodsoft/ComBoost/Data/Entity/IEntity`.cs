using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost.Data.Entity
{
    public interface IEntity<TKey> : IEntity, IEntityDTO<TKey>
    {

    }
}
