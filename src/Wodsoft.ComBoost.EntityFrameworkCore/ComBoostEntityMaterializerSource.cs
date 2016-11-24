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
        public ComBoostEntityMaterializerSource(IServiceProvider serviceProvider)
        {
            _Provider = serviceProvider;
        }

        private IServiceProvider _Provider;
        private static readonly MethodInfo _GetService = typeof(IServiceProvider).GetMethod("GetService");
        private static readonly MethodInfo _GetContext = typeof(DatabaseContextExtensions).GetMethod("GetDynamicContext");

        public override Expression CreateMaterializeExpression(IEntityType entityType, Expression valueBufferExpression, int[] indexMap = null)
        {
            BlockExpression expression = (BlockExpression)base.CreateMaterializeExpression(entityType, valueBufferExpression, indexMap);
            if (typeof(IEntity).IsAssignableFrom(entityType.ClrType))
            {
                var provider = Expression.Constant(_Provider);
                var currentDatabaseContext = Expression.Call(provider, _GetService, Expression.Constant(typeof(CurrentDatabaseContext)));
                var databaseContext = Expression.Property(Expression.Convert(currentDatabaseContext, typeof(CurrentDatabaseContext)), "Context");
                var entityContext = Expression.Call(_GetContext, databaseContext, Expression.Constant(entityType.ClrType));
                var property = Expression.Property(expression.Variables[0], typeof(IEntity).GetProperty("EntityContext"));
                var assign = Expression.Assign(property, Expression.Convert(entityContext, typeof(IEntityContext<>).MakeGenericType(entityType.ClrType)));
                var list = expression.Expressions.ToList();
                list.Insert(list.Count - 1, assign);
                expression = Expression.Block(expression.Variables, list);
            }
            return expression;
        }
    }
}
