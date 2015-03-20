using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace System.ServiceModel
{
    public sealed class ServiceEntityQueryProvider<TEntity> : IQueryProvider where TEntity : class, IEntity, new()
    {
        internal IEntityService<TEntity> Service;

        internal ServiceEntityQueryProvider(IEntityService<TEntity> service)
        {
            Service = service;
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            if (!typeof(TEntity).IsAssignableFrom(typeof(TElement)))
                throw new NotSupportedException();
            return (IQueryable<TElement>)new ServiceEntityQuery<TEntity>(this, expression);
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return new ServiceEntityQuery<TEntity>(this, expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            if (!typeof(TResult).IsAssignableFrom(typeof(TResult)))
                throw new NotSupportedException();
            return (TResult)(object)Service.QuerySingle(expression);
        }

        public object Execute(Expression expression)
        {
            return Service.QuerySingle(expression);
        }
    }
}
