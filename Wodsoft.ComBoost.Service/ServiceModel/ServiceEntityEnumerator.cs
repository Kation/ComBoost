using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ServiceModel
{
    public class ServiceEntityEnumerator<TEntity> : IEnumerator<TEntity> where TEntity : class, IEntity, new()
    {
        private IEntityService<TEntity> _Service;
        private Guid[] _Keys;
        private int _Current;

        internal ServiceEntityEnumerator(IEntityService<TEntity> service, Guid[] keys)
        {
            _Service = service;
            _Keys = keys;
        }

        private TEntity _Item;
        public TEntity Current
        {
            get
            {
                if (_Item == null)
                    _Item = _Service.GetEntity(_Keys[_Current]);
                return _Item;
            }
        }

        public void Dispose()
        {
            _Service = null;
            _Keys = null;
        }

        object Collections.IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        public bool MoveNext()
        {
            if (_Current + 1 == _Keys.Length)
                return false;
            _Current++;
            return true;
        }

        public void Reset()
        {
            _Current = 0;
        }
    }
}
