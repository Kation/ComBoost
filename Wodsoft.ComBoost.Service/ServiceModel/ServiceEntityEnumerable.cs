using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ServiceModel
{
    public class ServiceEntityEnumerable<TEntity> : IEnumerable<TEntity> where TEntity : class, IEntity, new()
    {
        private ServiceEntityEnumerator<TEntity> _Enumerator;
        internal ServiceEntityEnumerable(IEntityService<TEntity> service, Guid[] keys)
        {
            _Enumerator = new ServiceEntityEnumerator<TEntity>(service, keys);
        }

        public IEnumerator<TEntity> GetEnumerator()
        {
            return _Enumerator;
        }

        Collections.IEnumerator Collections.IEnumerable.GetEnumerator()
        {
            return _Enumerator;
        }
    }
}
