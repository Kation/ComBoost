using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Data.Entity
{
    public class WrappedQueryable<T, M> : IQueryable<T>
        where T : IEntity
        where M : IEntity, T
    {
        public WrappedQueryable(IQueryable<M> queryable)
        {
            if (queryable == null)
                throw new ArgumentNullException(nameof(queryable));
            InnerQueryable = queryable;
        }

        public IQueryable<M> InnerQueryable { get; private set; }

        public Type ElementType { get { return typeof(T); } }

        public Expression Expression
        {
            get
            {
                return InnerQueryable.Expression;
            }
        }

        public System.Linq.IQueryProvider Provider { get { return InnerQueryable.Provider; } }

        public IEnumerator<T> GetEnumerator()
        {
            return (IEnumerator<T>)InnerQueryable.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return InnerQueryable.GetEnumerator();
        }
    }
}
