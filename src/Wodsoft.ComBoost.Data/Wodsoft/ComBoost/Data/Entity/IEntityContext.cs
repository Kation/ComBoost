using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity.Metadata;

namespace Wodsoft.ComBoost.Data.Entity
{
    public interface IEntityContext<T>
        where T : class, new()
    {
        IEntityMetadata Metadata { get; }
        IDatabaseContext Database { get; }

        void Add(T item);
        void AddRange(IEnumerable<T> items);
        T Create();
        void Update(T item);
        void UpdateRange(IEnumerable<T> items);
        void Remove(T item);
        void RemoveRange(IEnumerable<T> items);
        IQueryable<T> Query();
        Task<T[]> ToArrayAsync(IQueryable<T> query);
        Task<List<T>> ToListAsync(IQueryable<T> query);
        Task<T> SingleOrDefaultAsync(IQueryable<T> query, Expression<Func<T, bool>> expression);
        Task<T> SingleAsync(IQueryable<T> query, Expression<Func<T, bool>> expression);
        Task<T> FirstOrDefaultAsync(IQueryable<T> query, Expression<Func<T, bool>> expression);
        Task<T> FirstAsync(IQueryable<T> query, Expression<Func<T, bool>> expression);
        Task<T> LastOrDefaultAsync(IQueryable<T> query, Expression<Func<T, bool>> expression);
        Task<T> LastAsync(IQueryable<T> query, Expression<Func<T, bool>> expression);
        Task<int> CountAsync(IQueryable<T> query);
        Task<int> CountAsync(IQueryable<T> query, Expression<Func<T, bool>> expression);
    }
}
