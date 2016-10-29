using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Internal;
using System.Reflection;

namespace Wodsoft.ComBoost.Data.Entity
{
    public class ComBoostEntityMaterializerSource : EntityMaterializerSource
    {
        public ComBoostEntityMaterializerSource(IMemberMapper memberMapper, CurrentDatabaseContext current) : base(memberMapper)
        {
            _database = current.Context;
        }

        private IDatabaseContext _database;

        public override Expression CreateMaterializeExpression(IEntityType entityType, Expression valueBufferExpression, int[] indexMap = null)
        {
            BlockExpression expression = (BlockExpression)base.CreateMaterializeExpression(entityType, valueBufferExpression, indexMap);
            if (typeof(IEntity).IsAssignableFrom(entityType.ClrType))
            {
                var entityContext = _database.GetDynamicContext(entityType.ClrType);
                var property = Expression.Property(expression.Variables[0], typeof(IEntity).GetProperty("EntityContext"));
                var assign = Expression.Assign(property, Expression.Constant(entityContext));
                var list = expression.Expressions.ToList();
                list.Insert(list.Count - 1, assign);
                expression = Expression.Block(expression.Variables, list);
            }
            return expression;
        }
    }
}
