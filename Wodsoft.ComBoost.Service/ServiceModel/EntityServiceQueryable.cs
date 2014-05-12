using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace System.ServiceModel
{
    public class EntityServiceQueryable<TEntity> : IQueryable<TEntity> where TEntity : class, IEntity, new()
    {
        internal EntityServiceQueryable(EntityServiceQueryableProvider<TEntity> provider, Expression expression)
        {
            _Provider = provider;
            Expression = expression;
        }

        public IEnumerator<TEntity> GetEnumerator()
        {
            return _Provider.Service.Query(Expression).GetEnumerator();
        }

        Collections.IEnumerator Collections.IEnumerable.GetEnumerator()
        {
            return _Provider.Service.Query(Expression).GetEnumerator();
        }

        public Type ElementType
        {
            get { return typeof(TEntity); }
        }

        public Expression Expression { get; private set; }

        private EntityServiceQueryableProvider<TEntity> _Provider;
        public IQueryProvider Provider { get { return _Provider; } }
    }
}
