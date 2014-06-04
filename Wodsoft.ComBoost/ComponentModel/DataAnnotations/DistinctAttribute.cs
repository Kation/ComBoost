﻿using System;
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
        public override bool RequiresValidationContext { get { return true; } }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
                return null;
            IEntity entity = (IEntity)validationContext.ObjectInstance;
            dynamic entityContext;
            Type type = validationContext.ObjectType;
            while (true)
            {
                try
                {
                    entityContext = validationContext.GetService(type);
                    break;
                }
                catch
                {
                    type = type.BaseType;
                }
            }
            ParameterExpression parameter = Expression.Parameter(type);
            Expression left = Expression.NotEqual(Expression.Property(parameter, "Index"), Expression.Constant(entity.Index));
            Expression right = Expression.Equal(Expression.Property(parameter, validationContext.MemberName), Expression.Constant(value));
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
