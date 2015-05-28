using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace System.Data.Entity.Metadata
{
    /// <summary>
    /// Extensions for property objects.
    /// </summary>
    public static class PropertyExtension
    {
        /// <summary>
        /// Get the custom type from property info.
        /// </summary>
        /// <param name="propertyInfo">Property info.</param>
        /// <param name="customType">Custom type if exists.</param>
        /// <param name="isFileUpload">Is property used to upload.</param>
        /// <returns>Custom data type.</returns>
        public static CustomDataType GetCustomDataType(this PropertyInfo propertyInfo, out string customType, out bool isFileUpload)
        {
            CustomDataType type;
            customType = null;
            isFileUpload = false;
            var customDataType = propertyInfo.GetCustomAttribute<CustomDataTypeAttribute>();
            if (customDataType != null)
            {
                type = customDataType.Type;
                customType = customDataType.Custom;
                isFileUpload = customDataType.IsFileUpload;
            }
            else
            {
                Type propertyType = propertyInfo.PropertyType;
                ValueFilterAttribute filter = propertyInfo.GetCustomAttribute<ValueFilterAttribute>();
                if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    propertyType = propertyType.GetGenericArguments()[0];
                if (filter != null)
                {
                    type = CustomDataType.Other;
                    customType = "ValueFilter";
                }
                else if (propertyType == typeof(DateTime))
                    type = CustomDataType.Date;
                else if (propertyType == typeof(TimeSpan))
                    type = CustomDataType.Time;
                else if (propertyType == typeof(bool))
                    type = CustomDataType.Boolean;
                else if (propertyType == typeof(short) || propertyType == typeof(int) || propertyType == typeof(long))
                    type = CustomDataType.Integer;
                else if (propertyType == typeof(float) || propertyType == typeof(double))
                    type = CustomDataType.Number;
                else if (propertyType == typeof(decimal))
                    type = CustomDataType.Currency;
                else if (propertyType == typeof(byte[]))
                {
                    type = CustomDataType.File;
                    isFileUpload = true;
                }
                else if (propertyType.IsEnum)
                {
                    type = CustomDataType.Other;
                    customType = "Enum";
                }
                else if (propertyType.IsGenericType)
                {
                    type = CustomDataType.Other;
                    customType = "Collection";
                }
                else if (typeof(IEntity).IsAssignableFrom(propertyType))
                {
                    type = CustomDataType.Other;
                    customType = "Entity";
                }
                else
                {
                    type = CustomDataType.Default;
                }
            }
            return type;
        }

        /// <summary>
        /// Get the get delegate from property info.
        /// </summary>
        /// <param name="propertyInfo">Property info.</param>
        /// <returns>The get delegate made of labmda expression.</returns>
        public static Func<object, object> GetGetMethodDelegate(this PropertyInfo propertyInfo)
        {
            var parameterExpression = Expression.Parameter(typeof(object));
            var convertExpression = Expression.Convert(parameterExpression, propertyInfo.DeclaringType);
            var propertyExpression = Expression.Property(convertExpression, propertyInfo);
            convertExpression = Expression.Convert(propertyExpression, typeof(object));
            var lambda = Expression.Lambda<Func<object, object>>(convertExpression, parameterExpression).Compile();
            return lambda;
        }

        /// <summary>
        /// Get the set delegate from property info.
        /// </summary>
        /// <param name="propertyInfo">Property info.</param>
        /// <returns>The set delegate made of labmda expression.</returns>
        public static Action<object, object> GetSetMethodDelegate(this PropertyInfo propertyInfo)
        {
            var objParameter = Expression.Parameter(typeof(object));
            var valueParameter = Expression.Parameter(typeof(object));
            var objConverterParameter = Expression.Convert(objParameter, propertyInfo.DeclaringType);
            var valueConverterParameter = Expression.Convert(valueParameter, propertyInfo.PropertyType);
            var expression = Expression.Call(objConverterParameter, propertyInfo.GetSetMethod(), valueConverterParameter);
            var lambda = Expression.Lambda<Action<object, object>>(expression, objParameter, valueParameter).Compile();
            return lambda;
        }
    }
}
