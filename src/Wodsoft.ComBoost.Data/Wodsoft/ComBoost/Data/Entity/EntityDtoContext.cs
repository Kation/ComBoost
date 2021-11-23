using AutoMapper;
using AutoMapper.QueryableExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Linq;

namespace Wodsoft.ComBoost.Data.Entity
{
    public class EntityDtoContext<TKey, TEntity, TListDTO, TCreateDTO, TEditDTO> : IDTOContext<TKey, TListDTO, TCreateDTO, TEditDTO>
        where TEntity : class, IEntity<TKey>
        where TListDTO : IEntityDTO<TKey>
        where TCreateDTO : IEntityDTO<TKey>
        where TEditDTO : IEntityDTO<TKey>
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

        public async Task Remove(TKey id)
        {
            var entity = await _context.Query().Where(t => t.Id.Equals(id)).FirstOrDefaultAsync();
            if (entity != null)
            {
                _context.Remove(entity);
                await _context.Database.SaveAsync();
            }
        }

        public async Task RemoveRange(params TKey[] keys)
        {
            var entities = await _context.Query().Where(t => keys.Contains(t.Id)).ToArrayAsync();
            _context.RemoveRange(entities);
            await _context.Database.SaveAsync();
        }

        public async Task Update(TEditDTO item)
        {
            var entity = await _context.Query().Where(t => t.Id.Equals(item.Id)).FirstOrDefaultAsync();
            _mapper.Map(item, entity);
            _context.Update(entity);
            await _context.Database.SaveAsync();
            _mapper.Map(entity, item);
        }

        public async Task UpdateRange(IEnumerable<TEditDTO> items)
        {
            var source = items.ToArray();
            var keys = source.Select(t => t.Id).ToArray();
            var entities = await _context.Query().Where(t => keys.Contains(t.Id)).ToDictionaryAsync(t => t.Id, t => t);
            _context.UpdateRange(entities.Values);
            await _context.Database.SaveAsync();
            foreach (var item in source)
                _mapper.Map(entities[item.Id], item);
        }
    }
}
