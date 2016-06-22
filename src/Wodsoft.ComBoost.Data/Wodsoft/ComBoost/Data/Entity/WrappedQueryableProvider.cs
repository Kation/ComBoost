using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Data.Entity
{
    public class WrappedQueryableProvider<T, M> : System.Linq.IQueryProvider
        where T : IEntity
        where M : IEntity, T
    {
        public WrappedQueryableProvider(System.Linq.IQueryProvider queryProvider)
        {
            if (queryProvider == null)
                throw new ArgumentNullException(nameof(queryProvider));
            InnerQueryProvider = queryProvider;
        }

        public System.Linq.IQueryProvider InnerQueryProvider { get; private set; }

        public IQueryable CreateQuery(Expression expression)
        {
            return new WrappedQueryable<T, M>(this, expression);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            if (typeof(TElement) != typeof(T))
                throw new NotSupportedException("不支持的元素类型。");
            return (IQueryable<TElement>)new WrappedQueryable<T, M>(this, expression);
        }

        public object Execute(Expression expression)
        {
            return InnerQueryProvider.Execute(expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return InnerQueryProvider.Execute<TResult>(expression);
        }
    }
}
