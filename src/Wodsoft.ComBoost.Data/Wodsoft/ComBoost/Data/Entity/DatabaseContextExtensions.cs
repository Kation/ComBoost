using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;

namespace Wodsoft.ComBoost.Data.Entity
{
    public static class DatabaseContextExtensions
    {
        public static IEntityContext<T> GetMappedContext<T>(this IDatabaseContext context)
            where T : IEntity
        {
            Type type = typeof(T);
            type = context.SupportTypes.FirstOrDefault(t => type.IsAssignableFrom(type));
            if (type == null)
                throw new NotSupportedException("数据库上下文不支持该类型实体。");
            return (IEntityContext<T>)context.GetType().GetMethod("GetContext").MakeGenericMethod(type).Invoke(context, new object[0]);
        }
    }
}
