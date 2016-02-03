using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
                return null;
            IEntity entity = (IEntity)validationContext.ObjectInstance;
            dynamic entityContext;
            Type type = validationContext.ObjectType;
            while (type.Assembly.IsDynamic)
                type = type.BaseType;
            entityContext = validationContext.GetService(typeof(IEntityContext<>).MakeGenericType(type));
            if (entityContext == null)
                return null;
            if (value is string && IsCaseSensitive)
                value = ((string)value).ToLower();
            ParameterExpression parameter = Expression.Parameter(type);
            Expression left = Expression.NotEqual(Expression.Property(parameter, "Index"), Expression.Constant(entity.Index));
            Expression right;
            if (value is string && IsCaseSensitive)
                right = Expression.Equal(Expression.Call(Expression.Property(parameter, validationContext.MemberName), typeof(string).GetMethod("ToLower")), Expression.Constant(value));
            else
                right = Expression.Equal(Expression.Property(parameter, validationContext.MemberName), Expression.Constant(value));
            Expression expression = Expression.And(left, right);
            expression = Expression.Lambda(typeof(Func<,>).MakeGenericType(type, typeof(bool)), expression, parameter);
            object where = _QWhereMethod.MakeGenericMethod(type).Invoke(null, new[] { entityContext.Query(), expression });
            int count = (int)_QCountMethod.MakeGenericMethod(type).Invoke(null, new[] { where });
            if (count != 0)
                return new ValidationResult(string.Format("{0} can not be {1}, there is a same value in the database.", validationContext.MemberName, value));
            else
                return null;
        }

        private static MethodInfo _QWhereMethod = typeof(Linq.Queryable).GetMethods().Where(t => t.Name == "Where" && t.GetParameters().Length == 2).First();
        private static MethodInfo _QCountMethod = typeof(Linq.Queryable).GetMethods().Where(t => t.Name == "Count" && t.GetParameters().Length == 1).First();
    }
}
