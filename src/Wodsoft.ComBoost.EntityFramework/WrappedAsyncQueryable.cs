using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;

namespace Wodsoft.ComBoost.Data.Entity
{
    public abstract class WrappedAsyncQueryable : IQueryable
    {
        protected WrappedAsyncQueryable(Expression expression, WrappedAsyncQueryProvider queryProvider, Type elementType)
        {
            Expression = expression;
            _Provider = queryProvider;
            ElementType = elementType;
        }

        public Type ElementType { get; private set; }

        public Expression Expression { get; }

        private WrappedAsyncQueryProvider _Provider;
        public IQueryProvider Provider => _Provider;
        protected WrappedAsyncQueryProvider WrappedProvider => _Provider;

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _Provider.SourceProvider.CreateQuery(Expression).GetEnumerator();
        }
    }

    public class WrappedAsyncQueryable<T> : WrappedAsyncQueryable, IQueryable<T>, IOrderedQueryable<T>
    {
        public WrappedAsyncQueryable(IQueryable<T> queryable) : base(queryable.Expression, new WrappedAsyncQueryProvider(queryable.Provider, queryable.Expression), typeof(T))
        {

        }
        public WrappedAsyncQueryable(Expression expression, WrappedAsyncQueryProvider queryProvider) : base(expression, queryProvider, typeof(T))
        {

        }

        public IEnumerator<T> GetEnumerator()
        {
            return WrappedProvider.SourceProvider.CreateQuery<T>(Expression).GetEnumerator();
        }
    }
}
