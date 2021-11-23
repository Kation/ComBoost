using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity.Metadata;
using Wodsoft.ComBoost.Data.Linq;

namespace Wodsoft.ComBoost.Data.Entity
{
    public class EntityWrappedContext<T, M> : IEntityContext<T>
        where M : class, IEntity, T
        where T : class, IEntity
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
            InnerContext.AddRange(items.Cast<M>());
        }
                
        public T Create()
        {
            return InnerContext.Create();
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
            InnerContext.RemoveRange(items.Cast<M>());
        }

        public void Update(T item)
        {
            InnerContext.Update((M)item);
        }

        public void UpdateRange(IEnumerable<T> items)
        {
            InnerContext.UpdateRange(items.Cast<M>());
        }

        public IQueryable<T> Include<TProperty>(IQueryable<T> query, Expression<Func<T, TProperty>> expression)
        {
            ExpressionWrapper<T, M> wrapper = new ExpressionWrapper<T, M>();
            LambdaExpression newExpression = (LambdaExpression)wrapper.Visit(expression);
            var propertyType = newExpression.Type.GetGenericArguments()[1];
            var queryable = query.Unwrap<T, M>();
            queryable = (IQueryable<M>)InnerContext.GetType().GetMethod("Include").MakeGenericMethod(propertyType).Invoke(InnerContext, new object[] { queryable, newExpression });
            return queryable.Wrap<T, M>();
        }

        public Task<T> GetAsync(params object[] keys)
        {
            return InnerContext.GetAsync(keys).ContinueWith(task =>
            {
                if (task.Exception!=null)
                    System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(task.Exception).Throw();
                return (T)task.Result;
            });
        }
    }
}
