using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Data.Entity
{
    public class CacheEntityQueryable<TEntity> : ICacheEntityQueryable<TEntity> where TEntity : class, ICacheEntity, new()
    {

        public Guid[] GetKeys(DateTime updateTime)
        {
            throw new NotImplementedException();
        }

        public bool IsUpdated(Guid entityID, DateTime lastUpdateTime)
        {
            throw new NotImplementedException();
        }

        public void UpdateCache()
        {
            throw new NotImplementedException();
        }

        public async void UpdateCacheAsync()
        {
            throw new NotImplementedException();
        }

        public bool Add(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public TEntity Create()
        {
            throw new NotImplementedException();
        }

        public bool Remove(Guid entityID)
        {
            throw new NotImplementedException();
        }

        public bool Edit(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public TEntity GetEntity(Guid entityID)
        {
            throw new NotImplementedException();
        }

        public IQueryable<TEntity> Query()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TEntity> Query(string sql, params object[] parameters)
        {
            throw new NotImplementedException();
        }

        public IQueryable<TEntity> InParent(IQueryable<TEntity> queryable, Guid[] parents)
        {
            throw new NotImplementedException();
        }

        public IQueryable<TEntity> InParent(IQueryable<TEntity> queryable, string path, Guid id)
        {
            throw new NotImplementedException();
        }

        public bool Editable()
        {
            throw new NotImplementedException();
        }

        public bool Addable()
        {
            throw new NotImplementedException();
        }

        public bool Removeable()
        {
            throw new NotImplementedException();
        }

        public int Count()
        {
            throw new NotImplementedException();
        }

        public bool Contains(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public bool Contains(Guid id)
        {
            throw new NotImplementedException();
        }

        public IOrderedQueryable<TEntity> OrderBy(IQueryable<TEntity> queryable)
        {
            throw new NotImplementedException();
        }

        public IOrderedQueryable<TEntity> OrderBy()
        {
            throw new NotImplementedException();
        }


        public bool AddRange(IEnumerable<TEntity> entities)
        {
            throw new NotImplementedException();
        }
    }
}
