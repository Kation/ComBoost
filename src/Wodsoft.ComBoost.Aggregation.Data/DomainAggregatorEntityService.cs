using AutoMapper;
using AutoMapper.QueryableExtensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data;
using Wodsoft.ComBoost.Data.Entity;
using Wodsoft.ComBoost.Data.Entity.Metadata;
using Wodsoft.ComBoost.Data.Linq;

namespace Wodsoft.ComBoost.Aggregation.Data
{
    public class DomainAggregatorEntityService<TEntity, T> : DomainService
        where TEntity : class, IEntity
    {
        private IEntityContext<TEntity> _entityContext;
        private IMapper _mapper;

        public DomainAggregatorEntityService(IEntityContext<TEntity> entityContext, IMapper mapper)
        {
            _entityContext = entityContext ?? throw new ArgumentNullException(nameof(entityContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public Task<T?> GetAsync(string[] keys)
        {
            var metadata = EntityDescriptor.GetMetadata<T>();
            if (keys.Length != metadata.KeyProperties.Count)
                return Task.FromResult(default(T));
            Expression? expression = null;
            ParameterExpression parameter = Expression.Parameter(typeof(T));
            for(int i = 0; i < metadata.KeyProperties.Count; i++)
            {
                var property = metadata.KeyProperties[i];
                var value = TypeDescriptor.GetConverter(property.ClrType).ConvertFromString(keys[i]);
                var exp = Expression.Equal(Expression.Property(parameter, property.ClrName), Expression.Constant(value, property.ClrType));
                if (expression == null)
                    expression = exp;
                else
                    expression = Expression.AndAlso(expression, exp);
            }
            var predicate = _mapper.Map<Expression<Func<TEntity, bool>>>(Expression.Lambda<Func<T, bool>>(expression, parameter));
#pragma warning disable CS8619 // 值中的引用类型的为 Null 性与目标类型不匹配。
            return _entityContext.Query().Where(predicate).ProjectTo<T>(_mapper.ConfigurationProvider).FirstOrDefaultAsync();
#pragma warning restore CS8619 // 值中的引用类型的为 Null 性与目标类型不匹配。
        }
    }
}
