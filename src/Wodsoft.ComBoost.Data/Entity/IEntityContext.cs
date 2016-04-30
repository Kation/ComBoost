using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Data.Entity
{
    public interface IEntityContext<T>
    {
        IDatabaseContext Database { get; }
        void Add(T item);
        void AddRange(IEnumerable<T> items);
        T Create();
        void Update(T item);
        void Remove(T item);
        void RemoveRange(IEnumerable<T> items);
        IQueryable<T> Query();
        Task<T> GetAsync(object key);
        IOrderedQueryable<T> Order();
        IOrderedQueryable<T> Order(IQueryable<T> query);
        IQueryable<T> InParent(IQueryable<T> query, string path, object id);
        IQueryable<T> InParent(IQueryable<T> query, object[] parentIds);
        Task<T[]> ToArrayAsync(IQueryable<T> query);
        Task<List<T>> ToListAsync(IQueryable<T> query);
    }
}
