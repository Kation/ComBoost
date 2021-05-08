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
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Threading;

namespace Wodsoft.ComBoost.Data.Entity
{
    public class ComBoostEntityCollection<T> : IQueryableCollection<T>, IAsyncEnumerable<T>
        where T : IEntity
    {
        private INavigation _Navigation;
        private INavigation _Inverse;
        private EntityEntry _Entry;
        private IEntityContext<T> _Context;

        internal ComBoostEntityCollection(EntityEntry owner, IEntityContext<T> context, INavigation navigation, IQueryable<T> queryable, int count)
        {
            _Entry = owner;
            _Navigation = navigation;
            _Inverse = navigation.FindInverse();
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
            if (_Inverse.IsCollection())
            {
                _Entry.GetInfrastructure().AddToCollectionSnapshot(_Navigation, item);
            }
            else
            {
#if NETSTANDARD_20
                _Navigation.FindInverse().GetSetter().SetClrValue(item, _Entry.Entity);
#else
                _Navigation.FindInverse().PropertyInfo.SetValue(item, _Entry.Entity);
#endif
            }
            Count++;
        }

        public void Clear()
        {
            if (_Inverse.FindAnnotation("Required") != null)
            {

            }
            else
            {

            }
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
            if (_Inverse.IsCollection())
            {
                _Entry.GetInfrastructure().RemoveFromCollectionSnapshot(_Navigation, item);
            }
            else
            {
                if (_Inverse.GetGetter().GetClrValue(item) != _Entry.Entity)
                    return false;
#if NETSTANDARD2_0
                _Inverse.GetSetter().SetClrValue(item, null);
#else
                _Inverse.PropertyInfo.SetValue(item, null);
#endif
                if (item.IsNewCreated && _Inverse.FindAnnotation("Required") != null)
                    _Context.Remove(item);
                else
                    _Context.Update(item);
            }
            Count--;
            return true;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return InnerQueryable.GetEnumerator();
        }

#if NETSTANDARD2_0

        IAsyncEnumerator<T> IAsyncEnumerable<T>.GetEnumerator()
        {
            return ((IAsyncEnumerable<T>)InnerQueryable).GetEnumerator();
        }
#else
        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return ((IAsyncEnumerable<T>)InnerQueryable).GetAsyncEnumerator();
        }
#endif
    }
}
