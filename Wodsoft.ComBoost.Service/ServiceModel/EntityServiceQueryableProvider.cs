using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace System.ServiceModel
{
    internal class EntityServiceQueryableProvider<TEntity> : IQueryProvider where TEntity : class, IEntity, new()
    {
        public IEntityService<TEntity> Service { get; private set; }

        public EntityServiceQueryableProvider(IEntityService<TEntity> service)
        {
            if (service == null)
                throw new ArgumentNullException("service");
            Service = service;
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return (IQueryable<TElement>)new EntityServiceQueryable<TEntity>(this, expression);
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return new EntityServiceQueryable<TEntity>(this, expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return (TResult)(object)Service.QuerySingle(expression);
        }

        public object Execute(Expression expression)
        {
            return Service.QuerySingle(expression);
        }
    }
}
