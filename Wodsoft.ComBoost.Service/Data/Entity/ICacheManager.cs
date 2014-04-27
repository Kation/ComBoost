using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Data.Entity
{
    public interface ICacheManager<TEntity> : IEntityQueryable<TEntity> where TEntity : EntityBase, new()
    {
        bool IsCached(Guid entityID);
        void Clear();
        void Refresh();
        int CachedCount { get; }
    }
}
