using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Data.Entity
{
    public class WrappedQueryableProvider<T, M> : IAsyncQueryProvider
        where T : IEntity
        where M : IEntity, T
    {
        public WrappedQueryableProvider(IAsyncQueryProvider queryProvider)
        {
            if (queryProvider == null)
                throw new ArgumentNullException(nameof(queryProvider));
            InnerQueryProvider = queryProvider;
        }

        public IAsyncQueryProvider InnerQueryProvider { get; private set; }

        public IAsyncQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            if (typeof(TElement) != typeof(T))
                throw new NotSupportedException("不支持的元素类型。");
            if (typeof(IOrderedQueryable).IsAssignableFrom(expression.Type))
                return (IAsyncQueryable<TElement>)new WrappedOrderedQueryable<T, M>(this, expression);
            else
                return (IAsyncQueryable<TElement>)new WrappedQueryable<T, M>(this, expression);
        }

        public ValueTask<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken token)
        {
            return InnerQueryProvider.ExecuteAsync<TResult>(expression, token);
        }
    }
}
