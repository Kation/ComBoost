using AutoMapper;
using AutoMapper.QueryableExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data;
using Wodsoft.ComBoost.Data.Entity;
using Wodsoft.ComBoost.Data.Linq;

namespace Wodsoft.ComBoost.Aggregation.Data
{
    public class DomainAggregatorEntityService<TKey, TEntity, T> : DomainService
        where TEntity: class, IEntity<TKey>
    {
        private IEntityContext<TEntity> _entityContext;
        private IMapper _mapper;

        public DomainAggregatorEntityService(IEntityContext<TEntity> entityContext, IMapper mapper)
        {
            _entityContext = entityContext ?? throw new ArgumentNullException(nameof(entityContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public Task<T> GetAsync(TKey id)
        {
            return _entityContext.Query().Where(t => t.Id.Equals(id)).ProjectTo<T>(_mapper.ConfigurationProvider).FirstOrDefaultAsync();
        }
    }
}
