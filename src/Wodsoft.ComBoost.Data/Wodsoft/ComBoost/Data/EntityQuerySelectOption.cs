using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Data
{
    public class EntityQuerySelectOption<TEntity, TViewModel>
    {
        public EntityQuerySelectOption(Expression<Func<TEntity,TViewModel>> selectExpression)
        {
            Expression = selectExpression;
        }

        public Expression<Func<TEntity, TViewModel>> Expression { get; private set; }

        public IQueryable<TViewModel> Select(IQueryable<TEntity> queryable)
        {
            return queryable.Select(Expression);
        }
    }
}
