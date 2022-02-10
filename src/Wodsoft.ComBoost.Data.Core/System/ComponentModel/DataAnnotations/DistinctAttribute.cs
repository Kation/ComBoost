using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity;
using Wodsoft.ComBoost.Data.Entity.Metadata;
using Microsoft.Extensions.DependencyInjection;

namespace System.ComponentModel.DataAnnotations
{
    /// <summary>
    /// Property distinct attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class DistinctAttribute : ValidationAttribute
    {
        /// <summary>
        /// Get or set is case sensitive for string property.
        /// </summary>
        public bool IsCaseSensitive { get; set; }

        /// <summary>
        /// Gets a value that indicates whether the attribute requires validation context.
        /// </summary>
        public override bool RequiresValidationContext { get { return true; } }

        /// <summary>
        /// Validates the specified value with respect to the current validation attribute.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
                return null;
            if (validationContext.MemberName == null)
                return null;
            object entity = validationContext.ObjectInstance;
            Type type = validationContext.ObjectType;
            while (type.GetTypeInfo().Assembly.IsDynamic)
                type = type.GetTypeInfo().BaseType;
            var databaseContext = validationContext.GetRequiredService<IDatabaseContext>();
            var entityContext = databaseContext.GetDynamicContext(type);
            if (value is string && !IsCaseSensitive)
                value = ((string)value).ToLower();
            ParameterExpression parameter = Expression.Parameter(type);
            IEntityMetadata metadata = EntityDescriptor.GetMetadata(type);
            var property = metadata.GetProperty(validationContext.MemberName)!;
            Expression left = Expression.NotEqual(Expression.Property(parameter, property.ClrName), Expression.Constant(property.GetValue(entity)));
            Expression right;
            if (value is string && IsCaseSensitive)
                right = Expression.Equal(Expression.Property(parameter, validationContext.MemberName), Expression.Constant(value));
            else
                right = Expression.Equal(Expression.Call(Expression.Property(parameter, validationContext.MemberName), typeof(string).GetMethod("ToLower", Array.Empty<Type>())), Expression.Constant(value)); 
            Expression expression = Expression.And(left, right);
            expression = Expression.Lambda(typeof(Func<,>).MakeGenericType(type, typeof(bool)), expression, parameter);
            dynamic where = _QWhereMethod.MakeGenericMethod(type).Invoke(null, new[] { entityContext.Query(), expression });
            int count = ((Task<int>)entityContext.CountAsync(where)).Result; ;
            if (count != 0)
                if (ErrorMessage != null)
                    return new ValidationResult(ErrorMessage);
                else
                    return new ValidationResult(string.Format("{0}不能为“{1}”，因为数据库已存在同样的值。", validationContext.DisplayName, value));
            else
                return ValidationResult.Success;
        }

        private static MethodInfo _QWhereMethod = typeof(Queryable).GetMethods().Where(t => t.Name == "Where" && t.GetParameters().Length == 2).First();
        //private static MethodInfo _QCountMethod = typeof(Queryable).GetMethods().Where(t => t.Name == "Count" && t.GetParameters().Length == 1).First();
    }
}
