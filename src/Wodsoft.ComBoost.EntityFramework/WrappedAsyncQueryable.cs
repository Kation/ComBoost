using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;

namespace Wodsoft.ComBoost.Data.Entity
{
    public abstract class WrappedAsyncQueryable : IAsyncQueryable
    {
        public abstract Type ElementType { get; }

        public abstract Expression Expression { get; }

        public abstract IAsyncQueryProvider Provider { get; }
    }

    public class WrappedAsyncQueryable<T> : WrappedAsyncQueryable, IAsyncQueryable<T>, IOrderedAsyncQueryable<T>
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

        public override Type ElementType => typeof(T);

        public override Expression Expression { get; }

        private WrappedAsyncQueryProvider _Provider;
        public override IAsyncQueryProvider Provider => _Provider;

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            var expression = new WrappedAsyncExpressionVisitor(_Provider).Visit(Expression);
            return ((IAsyncEnumerable<T>)_Provider.SourceProvider.CreateQuery<T>(expression)).GetAsyncEnumerator(cancellationToken);
        }
    }
}
