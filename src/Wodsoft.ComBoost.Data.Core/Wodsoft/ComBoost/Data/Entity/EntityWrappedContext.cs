using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity.Metadata;

namespace Wodsoft.ComBoost.Data.Entity
{
    public class EntityWrappedContext<T, M> : IEntityContext<T>
        where M : IEntity, T
        where T : IEntity
    {
        public EntityWrappedContext(IEntityContext<M> context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            InnerContext = context;
        }

        public IEntityContext<M> InnerContext { get; private set; }

        public IDatabaseContext Database { get { return InnerContext.Database; } }

        public IEntityMetadata Metadata { get { return InnerContext.Metadata; } }

        public void Add(T item)
        {
            InnerContext.Add((M)item);
        }

        public void AddRange(IEnumerable<T> items)
        {
            InnerContext.AddRange((IEnumerable<M>)items);
        }

        public Task<int> CountAsync(IQueryable<T> query)
        {
            return InnerContext.CountAsync(query.Unwrap<T, M>());
        }

        protected virtual Expression<Func<M, TResult>> WrapExpression<TResult>(Expression<Func<T, TResult>> expression)
        {
            ExpressionWrapper<T, M> wrapper = new ExpressionWrapper<T, M>();
            return (Expression<Func<M, TResult>>)wrapper.Visit(expression);
        }

        public T Create()
        {
            return InnerContext.Create();
        }

        public async Task<T> FirstAsync(IQueryable<T> query)
        {
            return await InnerContext.FirstAsync(query.Unwrap<T, M>());
        }

        public async Task<T> FirstOrDefaultAsync(IQueryable<T> query)
        {
            return await InnerContext.FirstOrDefaultAsync(query.Unwrap<T, M>());
        }

        public IQueryable<T> Query()
        {
            return InnerContext.Query().Wrap<T, M>();
        }

        public void Remove(T item)
        {
            InnerContext.Remove((M)item);
        }

        public void RemoveRange(IEnumerable<T> items)
        {
            InnerContext.RemoveRange((IEnumerable<M>)items);
        }

        public async Task<T> SingleAsync(IQueryable<T> query)
        {
            return await InnerContext.SingleAsync(query.Unwrap<T, M>());
        }

        public async Task<T> SingleOrDefaultAsync(IQueryable<T> query)
        {
            return await InnerContext.SingleOrDefaultAsync(query.Unwrap<T, M>());
        }

        public async Task<T[]> ToArrayAsync(IQueryable<T> query)
        {
            return (await InnerContext.ToArrayAsync(query.Unwrap<T, M>())).Cast<T>().ToArray();
        }

        public async Task<List<T>> ToListAsync(IQueryable<T> query)
        {
            return (await InnerContext.ToListAsync(query.Unwrap<T, M>())).Cast<T>().ToList();
        }

        public void Update(T item)
        {
            InnerContext.Update((M)item);
        }

        public void UpdateRange(IEnumerable<T> items)
        {
            InnerContext.UpdateRange((IEnumerable<M>)items);
        }

        public IQueryable<T> Include<TProperty>(IQueryable<T> query, Expression<Func<T, TProperty>> expression)
        {
            var newExpression = WrapExpression(expression);
            return InnerContext.Include(query.Unwrap<T, M>(), newExpression).Wrap<T, M>();
        }

        public async Task<T> GetAsync(object key)
        {
            return (M)await InnerContext.GetAsync(key);
        }

        public Task ReloadAsync(T item)
        {
            return InnerContext.ReloadAsync((M)item);
        }

        public IQueryable<T> ExecuteQuery(string sql, params object[] parameters)
        {
            throw new NotSupportedException();
        }
    }
}
