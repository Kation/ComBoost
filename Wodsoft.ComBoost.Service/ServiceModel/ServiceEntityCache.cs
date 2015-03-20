using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ServiceModel
{
    public class ServiceEntityCache<TEntity> where TEntity : class, IEntity, new()
    {
        private Dictionary<Guid, TEntity> _Cache;
        private Dictionary<Guid, DateTime> _Expire;

        public ServiceEntityCache()
        {
            _Cache = new Dictionary<Guid, TEntity>();
            _Expire = new Dictionary<Guid, DateTime>();
        }

        public TEntity GetEntity(Guid id, out bool timeout)
        {
            timeout = true;
            if (!_Cache.ContainsKey(id))
                return null;
            if (_Expire[id] > DateTime.Now)
                timeout = false;
            return _Cache[id];
        }

        public void SetEntity(TEntity entity)
        {
            _Cache.Add(entity.Index, entity);
            _Expire.Add(entity.Index, DateTime.Now.AddMinutes(10));
        }

        public void UpdateEntity(TEntity entity)
        {
            _Expire[entity.Index] = DateTime.Now.AddMinutes(10);
        }

        public void RemoveEntity(Guid id)
        {
            _Cache.Remove(id);
            _Expire.Remove(id);
        }
    }
}
