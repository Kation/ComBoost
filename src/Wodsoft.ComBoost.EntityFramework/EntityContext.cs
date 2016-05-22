using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity;

namespace Wodsoft.ComBoost.EntityFramework
{
    public class EntityContext<T> : IEntityContext<T>
        where T : class, new()
    {
        public EntityContext(DatabaseContext database, DbSet<T> dbset)
        {

        }

        public IDatabaseContext Database
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void Add(T item)
        {
            throw new NotImplementedException();
        }

        public void AddRange(IEnumerable<T> items)
        {
            throw new NotImplementedException();
        }

        public T Create()
        {
            throw new NotImplementedException();
        }

        public Task<T> GetAsync(object key)
        {
            throw new NotImplementedException();
        }

        public IQueryable<T> InParent(IQueryable<T> query, object[] parentIds)
        {
            throw new NotImplementedException();
        }

        public IQueryable<T> InParent(IQueryable<T> query, string path, object id)
        {
            throw new NotImplementedException();
        }

        public bool IsNew(T item)
        {
            throw new NotImplementedException();
        }

        public IOrderedQueryable<T> Order()
        {
            throw new NotImplementedException();
        }

        public IOrderedQueryable<T> Order(IQueryable<T> query)
        {
            throw new NotImplementedException();
        }

        public IQueryable<T> Query()
        {
            throw new NotImplementedException();
        }

        public void Remove(T item)
        {
            throw new NotImplementedException();
        }

        public void RemoveRange(IEnumerable<T> items)
        {
            throw new NotImplementedException();
        }

        public Task<T[]> ToArrayAsync(IQueryable<T> query)
        {
            throw new NotImplementedException();
        }

        public Task<List<T>> ToListAsync(IQueryable<T> query)
        {
            throw new NotImplementedException();
        }

        public void Update(T item)
        {
            throw new NotImplementedException();
        }
    }
}
