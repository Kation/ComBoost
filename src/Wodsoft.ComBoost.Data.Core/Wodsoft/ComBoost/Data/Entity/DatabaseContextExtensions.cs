using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;

namespace Wodsoft.ComBoost.Data.Entity
{
    /// <summary>
    /// 数据库上下文扩展。
    /// </summary>
    public static class DatabaseContextExtensions
    {
        /// <summary>
        /// 获取包装过的实体上下文。
        /// 主要用于不完整的实体类型的使用。
        /// </summary>
        /// <typeparam name="T">不完整的实体类型。</typeparam>
        /// <param name="context">数据库上下文。</param>
        /// <returns>返回实体上下文。</returns>
        public static IEntityContext<T> GetWrappedContext<T>(this IDatabaseContext context)
            where T : class, IEntity
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            Type entityType = typeof(T);
            entityType = context.SupportTypes.FirstOrDefault(t => entityType.IsAssignableFrom(t))
                ?? throw new NotSupportedException("数据库上下文不支持该类型实体。");
            var getContextMethod = context.GetType().GetMethod("GetContext")
                ?? throw new InvalidOperationException("数据库上下文未实现 GetContext 方法。");
            var sourceContext = getContextMethod.MakeGenericMethod(entityType).Invoke(context, Array.Empty<object>())
                ?? throw new InvalidOperationException("无法创建实体上下文。");
            if (entityType == typeof(T))
                return (IEntityContext<T>)sourceContext;
            var wrapperType = typeof(EntityWrappedContext<,>).MakeGenericType(typeof(T), entityType);
            var wrappedContext = Activator.CreateInstance(wrapperType, sourceContext)
                ?? throw new InvalidOperationException("无法创建包装实体上下文。");
            return (IEntityContext<T>)wrappedContext;
        }

        /// <summary>
        /// 获取动态类型实体上下文。
        /// </summary>
        /// <param name="context">数据库上下文。</param>
        /// <param name="entityType">实体类型。</param>
        /// <returns>返回动态类型的实体上下文。</returns>
        public static dynamic GetDynamicContext(this IDatabaseContext context, Type entityType)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (entityType == null)
                throw new ArgumentNullException(nameof(entityType));
            if (entityType.GetTypeInfo().IsInterface)
                throw new ArgumentException("实体类型不能为接口。");
            if (entityType.GetTypeInfo().IsAbstract)
                throw new ArgumentException("实体类型不能为抽象的。");
            if (!typeof(IEntity).IsAssignableFrom(entityType))
                throw new ArgumentException("实体类型没有继承“IEntity”接口。");
            var getContextMethod = typeof(IDatabaseContext).GetMethod("GetContext")
                ?? throw new InvalidOperationException("IDatabaseContext 未定义 GetContext 方法。");
            return getContextMethod.MakeGenericMethod(entityType).Invoke(context, null)!;
        }
    }
}
