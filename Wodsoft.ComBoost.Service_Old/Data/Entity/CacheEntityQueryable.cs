using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wodsoft.Net.Service;

namespace System.Data.Entity
{
    [ServiceMode(ServiceMode.Single)]
    public class CacheEntityQueryable<TEntity> : IEntityQueryable<TEntity>, ICacheEntityQueryable<TEntity> where TEntity : CacheEntityBase, new()
    {
        public CacheEntityQueryable(DbContext dbContext)
            : base(dbContext)
        { }

        public override Guid? Add(TEntity entity)
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
    }
}
