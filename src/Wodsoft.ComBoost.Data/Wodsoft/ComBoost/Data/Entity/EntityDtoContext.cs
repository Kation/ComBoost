using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Data.Entity
{
    public class EntityDtoContext<TEntity, TList, TCreate, TEdit, TRemove> : IDTOContext<TList, TCreate, TEdit, TRemove>
        where TEntity : class, IEntity
    {
        private IEntityContext<TEntity> _context;
        private IMapper _mapper;

        public EntityDtoContext(IEntityContext<TEntity> entityContext, IMapper mapper)
        {
            _context = entityContext ?? throw new ArgumentNullException(nameof(entityContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task Add(TCreate item)
        {
            var entity = _mapper.Map<TCreate, TEntity>(item);
            _context.Add(entity);
            await _context.Database.SaveAsync();
            _mapper.Map(entity, item);
        }

        public async Task AddRange(IEnumerable<TCreate> items)
        {
            var source = items.ToArray();
            var entities = source.Select(t => _mapper.Map<TCreate, TEntity>(t)).ToArray();
            _context.AddRange(entities);
            await _context.Database.SaveAsync();
            for (int i = 0; i < source.Length; i++)
                _mapper.Map(entities[i], source[i]);
        }

        public IAsyncQueryable<TList> Query()
        {
            return _context.Query().ProjectTo<TList>(_mapper.ConfigurationProvider);
        }

        public Task Remove(TRemove item)
        {
            _context.Remove(_mapper.Map<TRemove, TEntity>(item));
            return _context.Database.SaveAsync();
        }

        public Task RemoveRange(IEnumerable<TRemove> items)
        {
            _context.AddRange(items.Select(t => _mapper.Map<TRemove, TEntity>(t)));
            return _context.Database.SaveAsync();
        }

        public async Task Update(TEdit item)
        {
            var entity = _mapper.Map<TEdit, TEntity>(item);
            _context.Update(entity);
            await _context.Database.SaveAsync();
            _mapper.Map(entity, item);
        }

        public async Task UpdateRange(IEnumerable<TEdit> items)
        {
            var source = items.ToArray();
            var entities = source.Select(t => _mapper.Map<TEdit, TEntity>(t)).ToArray();
            _context.UpdateRange(entities);
            await _context.Database.SaveAsync();
            for (int i = 0; i < source.Length; i++)
                _mapper.Map(entities[i], source[i]);
        }
    }
}
