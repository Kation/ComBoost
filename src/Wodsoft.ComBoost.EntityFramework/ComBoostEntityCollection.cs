using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Data.Entity.Infrastructure;

namespace Wodsoft.ComBoost.Data.Entity
{
    public class ComBoostEntityCollection<T> : IQueryableCollection<T>, IAsyncEnumerable<T>
        where T : IEntity
    {
        private DbCollectionEntry _Navigation;
        private DbEntityEntry _Entry;
        private IEntityContext<T> _Context;


        internal ComBoostEntityCollection(DbEntityEntry owner, IEntityContext<T> context, DbCollectionEntry navigation, IQueryable<T> queryable, int count)
        {
            _Entry = owner;
            _Navigation = navigation;
            _Context = context;
            InnerQueryable = queryable;
            Count = count;
        }

        public int Count { get; private set; }

        public IQueryable<T> InnerQueryable { get; private set; }

        public Type ElementType { get { return InnerQueryable.ElementType; } }

        public Expression Expression { get { return InnerQueryable.Expression; } }

        public bool IsReadOnly { get { return false; } }

        public System.Linq.IQueryProvider Provider { get { return InnerQueryable.Provider; } }

        public void Add(T item)
        {
            ((ICollection<T>)_Navigation.CurrentValue).Add(item);
            Count++;
        }

        public void Clear()
        {
            ((ICollection<T>)_Navigation.CurrentValue).Clear();
            Count = 0;
        }

        public bool Contains(T item)
        {
            return Queryable.Count(InnerQueryable, t => t.Index == item.Index) > 0;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            InnerQueryable.ToArray().CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return InnerQueryable.GetEnumerator();
        }

        public bool Remove(T item)
        {
            ((ICollection<T>)_Navigation.CurrentValue).Remove(item);
            Count--;
            return true;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return InnerQueryable.GetEnumerator();
        }

        IAsyncEnumerator<T> IAsyncEnumerable<T>.GetEnumerator()
        {
            return ((IAsyncEnumerable<T>)InnerQueryable).GetEnumerator();
        }
    }
}
