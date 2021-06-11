using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;

namespace Wodsoft.ComBoost.Data.Entity
{
    public class WrappedAsyncQueryable<T> : IAsyncQueryable<T>, IOrderedAsyncQueryable<T>
    {
        public WrappedAsyncQueryable(IQueryable<T> queryable)
        {
            _Provider = new WrappedAsyncQueryProvider(queryable.Provider, queryable.Expression);
            Expression = new WrappedAsyncQueryExpression(typeof(T));
        }
        public WrappedAsyncQueryable(Expression expression, WrappedAsyncQueryProvider queryProvider)
        {
            _Provider = queryProvider;
            Expression = expression;
        }

        public Type ElementType => typeof(T);

        public Expression Expression { get; }

        private WrappedAsyncQueryProvider _Provider;
        public IAsyncQueryProvider Provider => _Provider;

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            var expression = new WrappedAsyncExpressionVisitor(_Provider.SourceExpression).Visit(Expression);
            return ((IAsyncEnumerable<T>)_Provider.SourceProvider.CreateQuery<T>(expression)).GetAsyncEnumerator(cancellationToken);
        }
    }
}
