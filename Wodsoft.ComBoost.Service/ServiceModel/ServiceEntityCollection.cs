using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace System.ServiceModel
{
    public sealed class ServiceEntityCollection<TEntity> : ServiceEntityQuery<TEntity>, ICollection<TEntity> where TEntity : class, IEntity, new()
    {
        public ServiceEntityCollection(ServiceEntityQueryProvider<TEntity> provider, Expression expression)
            : base(provider, expression) { }

        public void Add(TEntity item)
        {
            throw new NotSupportedException();
        }

        public void Clear()
        {
            throw new NotSupportedException();
        }

        public bool Contains(TEntity item)
        {
            return this.Count(t => t.Index == item.Index) > 0;
        }

        public void CopyTo(TEntity[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { return this.Count(); }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public bool Remove(TEntity item)
        {
            throw new NotSupportedException();
        }

        public new Collections.IEnumerator GetEnumerator()
        {
            return base.GetEnumerator();
        }
    }
}
