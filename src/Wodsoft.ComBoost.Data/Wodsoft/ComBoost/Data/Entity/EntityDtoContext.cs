using AutoMapper;
using AutoMapper.QueryableExtensions;
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
        where TListDTO : class, IEntityDTO
        where TCreateDTO : class, IEntityDTO
        where TEditDTO : class, IEntityDTO
        where TRemoveDTO : class
    {
        private IEntityContext<TEntity> _context;
        private IMapper _mapper;

        public EntityDtoContext(IEntityContext<TEntity> entityContext, IMapper mapper)
        {
            _context = entityContext ?? throw new ArgumentNullException(nameof(entityContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task Add(TCreateDTO item)
        {
            var entity = _mapper.Map<TCreateDTO, TEntity>(item);
            _context.Add(entity);
            await _context.Database.SaveAsync();
            _mapper.Map(entity, item);
        }

        public async Task AddRange(IEnumerable<TCreateDTO> items)
        {
            var source = items.ToArray();
            var entities = source.Select(t => _mapper.Map<TCreateDTO, TEntity>(t)).ToArray();
            _context.AddRange(entities);
            await _context.Database.SaveAsync();
            for (int i = 0; i < source.Length; i++)
                _mapper.Map(entities[i], source[i]);
        }

        public IQueryable<TListDTO> Query()
        {
            return _context.Query().ProjectTo<TListDTO>(_mapper.ConfigurationProvider);
        }

        public async Task Remove(TRemoveDTO dto)
        {
            var mappedEntity = _mapper.Map<TEntity>(dto);
            var keyProperties = EntityDescriptor.GetMetadata<TEntity>().KeyProperties;
            var keys = new object[keyProperties.Count];
            for (int i = 0; i < keyProperties.Count; i++)
                keys[i] = keyProperties[i].GetValue(mappedEntity);
            var entity = await _context.GetAsync(keys);
            if (entity != null)
            {
                _context.Remove(entity);
                await _context.Database.SaveAsync();
            }
        }

        public async Task RemoveRange(params TRemoveDTO[] dtos)
        {
            var entities = dtos.Select(t => _mapper.Map<TEntity>(dtos)).ToArray();
            _context.RemoveRange(entities);
            await _context.Database.SaveAsync();
        }

        public async Task Update(TEditDTO item)
        {
            var mappedEntity = _mapper.Map<TEntity>(item);
            var keyProperties = EntityDescriptor.GetMetadata<TEntity>().KeyProperties;
            var keys = new object[keyProperties.Count];
            for (int i = 0; i < keyProperties.Count; i++)
                keys[i] = keyProperties[i].GetValue(mappedEntity);
            var entity = await _context.GetAsync(keys);
            _mapper.Map(item, entity);
            _context.Update(entity);
            await _context.Database.SaveAsync();
            _mapper.Map(entity, item);
        }

        public async Task UpdateRange(IEnumerable<TEditDTO> items)
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
            Expression expression = null;
            foreach (var key in keyList)
            {
                Expression equal = null;
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
                _mapper.Map(hashKey[item.Key], item.Value);
            _context.UpdateRange(entities.Values);
            await _context.Database.SaveAsync();
        }
    }
}
