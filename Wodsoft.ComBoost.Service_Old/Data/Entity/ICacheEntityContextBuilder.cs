using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Data.Entity
{
    public interface ICacheEntityContextBuilder : IEntityContextBuilder
    {
        ICacheEntityQueryable<TEntity> GetCacheContext<TEntity>() where TEntity : CacheEntityBase, new();
    }
}
