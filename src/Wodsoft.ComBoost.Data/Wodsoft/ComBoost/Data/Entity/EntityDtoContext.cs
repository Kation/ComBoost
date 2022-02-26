using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity.Metadata;
using Wodsoft.ComBoost.Data.Linq;

namespace Wodsoft.ComBoost.Data.Entity
{
    public class EntityDtoContext<TEntity, TListDTO, TCreateDTO, TEditDTO, TRemoveDTO> : IDTOContext<TListDTO, TCreateDTO, TEditDTO, TRemoveDTO>
        where TEntity : class, IEntity
        where TListDTO : class
        where TCreateDTO : class
        where TEditDTO : class
        where TRemoveDTO : class
    {
        private IEntityContext<TEntity> _context;
        private IMapper _mapper;
        private EntityDtoContextOptions<TEntity, TListDTO, TCreateDTO, TEditDTO, TRemoveDTO> _options;

        public EntityDtoContext(IEntityContext<TEntity> entityContext, IMapper mapper, IOptions<EntityDtoContextOptions<TEntity, TListDTO, TCreateDTO, TEditDTO, TRemoveDTO>> options)
        {
            _context = entityContext ?? throw new ArgumentNullException(nameof(entityContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task Add(TCreateDTO item)
        {
            var entity = _mapper.Map<TCreateDTO, TEntity>(item);
            _options.OnAddMapped?.Invoke(entity, item);
            _context.Add(entity);
            await _context.Database.SaveAsync();
            _mapper.Map(entity, item);
            _options.OnAdded?.Invoke(entity, item);
        }

        public async Task AddRange(IEnumerable<TCreateDTO> items)
        {
            var source = items.ToArray();
            var entities = source.Select(t =>
            {
                var entity = _mapper.Map<TCreateDTO, TEntity>(t);
                _options.OnAddMapped?.Invoke(entity, t);
                return entity;
            }).ToArray();
            _context.AddRange(entities);
            await _context.Database.SaveAsync();
            for (int i = 0; i < source.Length; i++)
            {
                _mapper.Map(entities[i], source[i]);
                _options.OnAdded?.Invoke(entities[i], source[i]);
            }
        }

        public IQueryable<TListDTO> Query()
        {
            return _context.Query().ProjectTo<TListDTO>(_mapper.ConfigurationProvider);
        }

        public async Task Remove(TRemoveDTO item)
        {
            var mappedEntity = _mapper.Map<TEntity>(item);
            var keyProperties = EntityDescriptor.GetMetadata<TEntity>().KeyProperties;
            var keys = new object[keyProperties.Count];
            for (int i = 0; i < keyProperties.Count; i++)
                keys[i] = keyProperties[i].GetValue(mappedEntity);
            var entity = await _context.GetAsync(keys);
            if (entity != null)
            {
                _options.OnPreRemove?.Invoke(entity, item);
                _context.Remove(entity);
                await _context.Database.SaveAsync();
                _options.OnRemoved?.Invoke(entity, item);
            }
        }

        public async Task RemoveRange(params TRemoveDTO[] items)
        {
            var keyProperties = EntityDescriptor.GetMetadata<TEntity>().KeyProperties;
            var keyList = new List<object[]>();
            var hashKey = new Dictionary<int, TRemoveDTO>();
            foreach (var item in items)
            {
                var mappedEntity = _mapper.Map<TEntity>(item);
                var keys = new object[keyProperties.Count];
                int hash = 0;
                for (int i = 0; i < keyProperties.Count; i++)
                {
                    var value = keyProperties[i].GetValue(mappedEntity);
                    keys[i] = value;
                    hash += value.GetHashCode();
                }
                hashKey.Add(hash, item);
                keyList.Add(keys);
            }
            ParameterExpression parameter = Expression.Parameter(typeof(TEntity), "entity");
            Expression? expression = null;
            foreach (var key in keyList)
            {
                Expression? equal = null;
                for (int i = 0; i < keyProperties.Count; i++)
                {
                    var e = Expression.Equal(Expression.Property(parameter, keyProperties[0].ClrName), Expression.Constant(key[i], keyProperties[i].ClrType));
                    if (i == 0)
                        equal = e;
                    else
                        equal = Expression.AndAlso(equal, e);
                }
                if (expression == null)
                    expression = equal;
                else
                    expression = Expression.OrElse(expression, equal);
            }
            var predicate = Expression.Lambda<Func<TEntity, bool>>(expression, parameter);
            var entities = await _context.Query().Where(predicate).ToDictionaryAsync(t =>
            {
                int hash = 0;
                for (int i = 0; i < keyProperties.Count; i++)
                    hash += keyProperties[i].GetValue(t).GetHashCode();
                return hash;
            }, t => t);
            foreach (var item in entities)
            {
                _options.OnPreRemove?.Invoke(item.Value, hashKey[item.Key]);
                _context.Remove(item.Value);
            }
            await _context.Database.SaveAsync();
            if (_options.OnRemoved != null)
                foreach (var item in entities)
                {
                    _options.OnRemoved.Invoke(item.Value, hashKey[item.Key]);
                }
        }

        public async Task Update(TEditDTO item)
        {
            var mappedEntity = _mapper.Map<TEntity>(item);
            var keyProperties = EntityDescriptor.GetMetadata<TEntity>().KeyProperties;
            var keys = new object[keyProperties.Count];
            for (int i = 0; i < keyProperties.Count; i++)
                keys[i] = keyProperties[i].GetValue(mappedEntity);
            var entity = await _context.GetAsync(keys);
            if (entity == null)
                throw new ResourceNotFoundException("Entity does not exists.");
            _options.OnPreUpdateMap?.Invoke(entity, item);
            _mapper.Map(item, entity);
            _options.OnUpdateMapped?.Invoke(entity, item);
            _context.Update(entity);
            await _context.Database.SaveAsync();
            _mapper.Map(entity, item);
            _options.OnUpdated?.Invoke(entity, item);
        }

        public async Task UpdateRange(params TEditDTO[] items)
        {
            var keyProperties = EntityDescriptor.GetMetadata<TEntity>().KeyProperties;
            var keyList = new List<object[]>();
            var hashKey = new Dictionary<int, TEditDTO>();
            foreach (var item in items)
            {
                var mappedEntity = _mapper.Map<TEntity>(item);
                var keys = new object[keyProperties.Count];
                int hash = 0;
                for (int i = 0; i < keyProperties.Count; i++)
                {
                    var value = keyProperties[i].GetValue(mappedEntity);
                    keys[i] = value;
                    hash += value.GetHashCode();
                }
                hashKey.Add(hash, item);
                keyList.Add(keys);
            }
            ParameterExpression parameter = Expression.Parameter(typeof(TEntity), "entity");
            Expression? expression = null;
            foreach (var key in keyList)
            {
                Expression? equal = null;
                for (int i = 0; i < keyProperties.Count; i++)
                {
                    var e = Expression.Equal(Expression.Property(parameter, keyProperties[0].ClrName), Expression.Constant(key[i], keyProperties[i].ClrType));
                    if (i == 0)
                        equal = e;
                    else
                        equal = Expression.AndAlso(equal, e);
                }
                if (expression == null)
                    expression = equal;
                else
                    expression = Expression.OrElse(expression, equal);
            }
            var predicate = Expression.Lambda<Func<TEntity, bool>>(expression, parameter);
            var entities = await _context.Query().Where(predicate).ToDictionaryAsync(t =>
            {
                int hash = 0;
                for (int i = 0; i < keyProperties.Count; i++)
                    hash += keyProperties[i].GetValue(t).GetHashCode();
                return hash;
            }, t => t);
            foreach (var item in entities)
            {
                _options.OnPreUpdateMap?.Invoke(item.Value, hashKey[item.Key]);
                _mapper.Map(hashKey[item.Key], item.Value);
                _options.OnUpdateMapped?.Invoke(item.Value, hashKey[item.Key]);
            }
            _context.UpdateRange(entities.Values);
            await _context.Database.SaveAsync();
            if (_options.OnUpdated != null)
                foreach (var item in entities)
                {
                    _options.OnUpdated.Invoke(item.Value, hashKey[item.Key]);
                }
        }
    }
}
