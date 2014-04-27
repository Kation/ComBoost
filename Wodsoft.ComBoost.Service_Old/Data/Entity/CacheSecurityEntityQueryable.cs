using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Data.Entity
{
    public class CacheSecurityEntityQueryable<TEntity> : SecurityEntityQueryable<TEntity>, ICacheEntityQueryable<TEntity> where TEntity : CacheEntityBase, new()
    {
        public CacheSecurityEntityQueryable(DbContext dbContext)
            : base(dbContext)
        { }

        public override bool Add(TEntity entity)
        {
            entity.UpdateTime = DateTime.Now;
            return base.Add(entity);
        }

        public override bool Edit(TEntity entity)
        {
            entity.UpdateTime = DateTime.Now;
            return base.Edit(entity);
        }

        public virtual bool IsUpdated(Guid entityID, DateTime lastUpdateTime)
        {
            return DbSet.Count(t => t.Index == entityID && t.UpdateTime > lastUpdateTime) > 0;
        }

        public virtual Guid[] GetKeys(DateTime updateTime)
        {
            return DbSet.Where(t => t.UpdateTime > updateTime).Select(t => t.Index).ToArray();
        }

        public virtual Guid[] InParentID(Guid[] parents)
        {
            throw new NotImplementedException();
        }
    }
}
