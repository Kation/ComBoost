using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity.Metadata;

namespace Wodsoft.ComBoost.Data.Entity
{
    public interface IEntityQueryContext<out T>
        where T : IEntity
    {
        IDatabaseContext Database { get; }

        IEntityMetadata Metadata { get; }

        IQueryable<T> Query();
    }
}
