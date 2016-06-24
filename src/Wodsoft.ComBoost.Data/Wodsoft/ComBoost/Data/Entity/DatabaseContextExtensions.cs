using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;

namespace Wodsoft.ComBoost.Data.Entity
{
    public static class DatabaseContextExtensions
    {
        public static IEntityContext<T> GetWrappedContext<T>(this IDatabaseContext context)
            where T : IEntity
        {
            Type type = typeof(T);
            type = context.SupportTypes.FirstOrDefault(t => type.IsAssignableFrom(t));
            if (type == null)
                throw new NotSupportedException("数据库上下文不支持该类型实体。");
            var sourceContext = context.GetType().GetMethod("GetContext").MakeGenericMethod(type).Invoke(context, new object[0]);
            if (type == typeof(T))
                return (IEntityContext<T>)sourceContext;
            var wrapperType = typeof(EntityWrappedContext<,>).MakeGenericType(typeof(T), type);
            var wrappedContext = Activator.CreateInstance(wrapperType, sourceContext);
            return (IEntityContext<T>)wrappedContext;
        }
    }
}
