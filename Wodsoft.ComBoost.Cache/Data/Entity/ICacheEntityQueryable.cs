using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Data.Entity
{
    public interface ICacheEntityQueryable<TEntity> : IEntityQueryable<TEntity> where TEntity : class, ICacheEntity, new()
    {
        Guid[] GetKeys(DateTime updateTime);

        bool IsUpdated(Guid entityID, DateTime lastUpdateTime);

        void UpdateCache();

        void UpdateCacheAsync();
    }
}
