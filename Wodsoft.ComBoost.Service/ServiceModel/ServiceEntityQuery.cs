using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace System.ServiceModel
{
    public class ServiceEntityQuery<TEntity> : IQueryable<TEntity> where TEntity : class, IEntity, new()
    {
        private ServiceEntityQueryProvider<TEntity> _Provider;
        internal ServiceEntityQuery(ServiceEntityQueryProvider<TEntity> provider, Expression expression)
        {
            _Provider = provider;
            Expression = expression;
            ElementType = typeof(TEntity);
        }

        public IEnumerator<TEntity> GetEnumerator()
        {
            return _Provider.Service.Query(Expression).GetEnumerator();
        }

        Collections.IEnumerator Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Type ElementType { get; private set; }

        public Expression Expression { get; private set; }

        public IQueryProvider Provider { get { return _Provider; } }
    }
}
