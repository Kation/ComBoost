using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace System.ServiceModel
{
    public abstract class EntityServiceBase<TEntity> : IEntityService<TEntity> where TEntity : class, IEntity, new()
    {
        private ServiceEntityCache<TEntity> _Cache;
        private Type _Type;

        public EntityServiceBase()
        {
            _Cache = new ServiceEntityCache<TEntity>();
            _Type = typeof(TEntity);
        }

        protected void MapToEntity(TEntity entity, Dictionary<string, object> values)
        {
            Parallel.ForEach(values, kv =>
            {
                _Type.GetProperty(kv.Key).SetValue(entity, kv.Value);
            });
        }

        public bool Authenticate(string data)
        {
            bool result = AuthenticateCore(data);
            _IsAuthenticated = result;
            _Addable = null;
            _Editable = null;
            _Readable = null;
            _Removeable = null;
            return result;
        }
        protected abstract bool AuthenticateCore(string data);

        private bool? _IsAuthenticated;
        public bool IsAuthenticated()
        {
            if (_IsAuthenticated == null)
            {
                _IsAuthenticated = IsAuthenticatedCore();
                _Addable = null;
                _Editable = null;
                _Readable = null;
                _Removeable = null;
            }
            return _IsAuthenticated.Value;
        }
        protected abstract bool IsAuthenticatedCore();

        public TEntity GetEntity(Guid id)
        {
            if (!Readable())
                throw new NotSupportedException("Service doesn't support.");
            bool timeout;
            TEntity entity = _Cache.GetEntity(id, out timeout);
            if (entity != null)
            {
                if (timeout)
                    ReloadAsync(id).Start();
                return entity;
            }
            else
                entity = ServiceEntityProxyBuilder.CreateEntityProxy<TEntity>();
            var values = GetEntityCore(id);
            if (values == null)
                return null;
            MapToEntity(entity, values);
            return entity;
        }
        protected abstract Dictionary<string, object> GetEntityCore(Guid id);

        public async Task<TEntity> GetEntityAsync(Guid id)
        {
            if (!Readable())
                throw new NotSupportedException("Service doesn't support.");
            bool timeout;
            TEntity entity = _Cache.GetEntity(id, out timeout);
            if (entity != null)
            {
                if (timeout)
                    ReloadAsync(id).Start();
                return entity;
            }
            else
                entity = ServiceEntityProxyBuilder.CreateEntityProxy<TEntity>();
            var values = await GetEntityCoreAsync(id);
            if (values == null)
                return null;
            MapToEntity(entity, values);
            return entity;
        }
        protected abstract Task<Dictionary<string, object>> GetEntityCoreAsync(Guid id);

        public bool Add(TEntity entity)
        {
            if (!Addable())
                throw new NotSupportedException("Service doesn't support.");
            if (entity.GetType() == typeof(TEntity))
                throw new NotSupportedException("Entity is not create by service.");
            bool result = AddCore(entity);
            if (result)
                _Cache.SetEntity(entity);
            return result;
        }
        protected abstract bool AddCore(TEntity entity);

        public async Task<bool> AddAsync(TEntity entity)
        {
            if (!Addable())
                throw new NotSupportedException("Service doesn't support.");
            if (entity.GetType() == typeof(TEntity))
                throw new NotSupportedException("Entity is not create by service.");
            bool result = await AddCoreAsync(entity);
            if (result)
                _Cache.SetEntity(entity);
            return result;
        }
        protected abstract Task<bool> AddCoreAsync(TEntity entity);

        public bool AddRange(TEntity[] entities)
        {
            if (!Addable())
                throw new NotSupportedException("Service doesn't support.");
            if (entities.Any(t => t.GetType() == typeof(TEntity)))
                throw new NotSupportedException("One of entity in array is not create by service.");
            bool result = AddRangeCore(entities);
            if (result)
                foreach (var entity in entities)
                    _Cache.SetEntity(entity);
            return result;
        }
        protected abstract bool AddRangeCore(TEntity[] entities);

        public async Task<bool> AddRangeAsync(TEntity[] entities)
        {
            if (!Addable())
                throw new NotSupportedException("Service doesn't support.");
            if (entities.Any(t => t.GetType() == typeof(TEntity)))
                throw new NotSupportedException("One of entity in array is not create by service.");
            bool result = await AddRangeCoreAsync(entities);
            if (result)
                foreach (var entity in entities)
                    _Cache.SetEntity(entity);
            return result;
        }
        protected abstract Task<bool> AddRangeCoreAsync(TEntity[] entities);

        public TEntity Create()
        {
            return ServiceEntityProxyBuilder.CreateEntityProxy<TEntity>();
        }

        public bool Edit(TEntity entity)
        {
            if (!Editable())
                throw new NotSupportedException("Service doesn't support.");
            if (entity.GetType() == typeof(TEntity))
                throw new NotSupportedException("Entity is not create by service.");
            return EditCore(entity);
        }
        protected abstract bool EditCore(TEntity entity);

        public Task<bool> EditAsync(TEntity entity)
        {
            if (!Editable())
                throw new NotSupportedException("Service doesn't support.");
            if (entity.GetType() == typeof(TEntity))
                throw new NotSupportedException("Entity is not create by service.");
            return EditCoreAsync(entity);
        }
        protected abstract Task<bool> EditCoreAsync(TEntity entity);

        public bool Remove(Guid id)
        {
            if (!Removeable())
                throw new NotSupportedException("Service doesn't support.");
            bool result = RemoveCore(id);
            if (result)
                _Cache.RemoveEntity(id);
            return result;
        }
        protected abstract bool RemoveCore(Guid id);

        public async Task<bool> RemoveAsync(Guid id)
        {
            if (!Removeable())
                throw new NotSupportedException("Service doesn't support.");
            bool result = await RemoveCoreAsync(id);
            if (result)
                _Cache.RemoveEntity(id);
            return result;
        }
        protected abstract Task<bool> RemoveCoreAsync(Guid id);

        public bool RemoveRange(Guid[] keys)
        {
            if (!Removeable())
                throw new NotSupportedException("Service doesn't support.");
            bool result = RemoveRangeCore(keys);
            if (result)
                foreach (var id in keys)
                    _Cache.RemoveEntity(id);
            return result;
        }
        protected abstract bool RemoveRangeCore(Guid[] keys);

        public async Task<bool> RemoveRangeAsync(Guid[] keys)
        {
            if (!Removeable())
                throw new NotSupportedException("Service doesn't support.");
            bool result = await RemoveRangeCoreAsync(keys);
            if (result)
                foreach (var id in keys)
                    _Cache.RemoveEntity(id);
            return result;
        }
        protected abstract Task<bool> RemoveRangeCoreAsync(Guid[] keys);

        public IEnumerable<TEntity> Query(Expression expression)
        {
            if (!Readable())
                throw new NotSupportedException("Service doesn't support.");
            var result = QueryCore(expression);
            if (result == null)
                return null;
            return new ServiceEntityEnumerable<TEntity>(this, result);
        }
        protected abstract Guid[] QueryCore(Expression expression);

        public async Task<IEnumerable<TEntity>> QueryAsync(Expression expression)
        {
            if (!Readable())
                throw new NotSupportedException("Service doesn't support.");
            var result = await QueryCoreAsync(expression);
            if (result == null)
                return null;
            return new ServiceEntityEnumerable<TEntity>(this, result);
        }
        protected abstract Task<Guid[]> QueryCoreAsync(Expression expression);

        public TEntity QuerySingle(Expression expression)
        {
            if (!Readable())
                throw new NotSupportedException("Service doesn't support.");
            Guid? id = QuerySingleCore(expression);
            if (!id.HasValue)
                return null;
            return GetEntity(id.Value);
        }
        protected abstract Guid? QuerySingleCore(Expression expression);

        public async Task<TEntity> QuerySingleAsync(Expression expression)
        {
            if (!Readable())
                throw new NotSupportedException("Service doesn't support.");
            Guid? id = await QuerySingleCoreAsync(expression);
            if (!id.HasValue)
                return null;
            return await GetEntityAsync(id.Value);
        }
        protected abstract Task<Guid?> QuerySingleCoreAsync(Expression expression);

        public IEnumerable<TEntity> QuerySql(string sql, params object[] parameters)
        {
            if (!Readable())
                throw new NotSupportedException("Service doesn't support.");
            var result = QuerySqlCore(sql, parameters);
            if (result == null)
                return null;
            return new ServiceEntityEnumerable<TEntity>(this, result);
        }
        protected abstract Guid[] QuerySqlCore(string sql, params object[] parameters);

        public async Task<IEnumerable<TEntity>> QuerySqlAsync(string sql, params object[] parameters)
        {
            if (!Readable())
                throw new NotSupportedException("Service doesn't support.");
            var result = await QuerySqlCoreAsync(sql, parameters);
            if (result == null)
                return null;
            return new ServiceEntityEnumerable<TEntity>(this, result);
        }
        protected abstract Task<Guid[]> QuerySqlCoreAsync(string sql, params object[] parameters);

        public int Count()
        {
            if (!Readable())
                throw new NotSupportedException("Service doesn't support.");
            return CountCore();
        }
        protected abstract int CountCore();

        public Task<int> CountAsync()
        {
            if (!Readable())
                throw new NotSupportedException("Service doesn't support.");
            return CountCoreAsync();
        }
        protected abstract Task<int> CountCoreAsync();

        public bool Contains(Guid id)
        {
            if (!Readable())
                throw new NotSupportedException("Service doesn't support.");
            return ContainsCore(id);
        }
        protected abstract bool ContainsCore(Guid id);

        public Task<bool> ContainsAsync(Guid id)
        {
            if (!Readable())
                throw new NotSupportedException("Service doesn't support.");
            return ContainsCoreAsync(id);
        }
        protected abstract Task<bool> ContainsCoreAsync(Guid id);

        public bool Reload(Guid id)
        {
            if (!Readable())
                throw new NotSupportedException("Service doesn't support.");
            bool timeout;
            TEntity entity = _Cache.GetEntity(id, out timeout);
            if (entity == null)
                return false;
            var values = GetEntityCore(id);
            if (values == null)
            {
                _Cache.RemoveEntity(id);
                return false;
            }
            MapToEntity(entity, values);
            _Cache.UpdateEntity(entity);
            return true;
        }

        public async Task<bool> ReloadAsync(Guid id)
        {
            if (!Readable())
                throw new NotSupportedException("Service doesn't support.");
            bool timeout;
            TEntity entity = _Cache.GetEntity(id, out timeout);
            if (entity == null)
                return false;
            var values = await GetEntityCoreAsync(id);
            if (values == null)
            {
                _Cache.RemoveEntity(id);
                return false;
            }
            MapToEntity(entity, values);
            _Cache.UpdateEntity(entity);
            return true;
        }

        private bool? _Editable;
        public bool Editable()
        {
            if (_Editable == null)
                _Editable = EditableCore();
            return _Editable.Value;
        }
        protected abstract bool EditableCore();

        private bool? _Addable;
        public bool Addable()
        {
            if (_Addable == null)
                _Addable = AddableCore();
            return _Addable.Value;
        }
        protected abstract bool AddableCore();

        private bool? _Removeable;
        public bool Removeable()
        {
            if (_Removeable == null)
                _Removeable = RemoveableCore();
            return _Removeable.Value;
        }
        protected abstract bool RemoveableCore();

        private bool? _Readable;
        public bool Readable()
        {
            if (_Readable == null)
                _Readable = ReadableCore();
            return _Readable.Value;
        }
        protected abstract bool ReadableCore();
    }
}
