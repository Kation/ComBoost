using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Data.Entity
{
    /// <summary>
    /// 带缓存的实体上下文建造者
    /// </summary>
    public class CacheEntityContextBuilder : ICacheEntityContextBuilder
    {
        public DbContext DbContext { get; private set; }

        public CacheEntityContextBuilder(DbContext dbContext)
        {
            DbContext = dbContext;

            List<Type> types = new List<Type>();
            foreach (var property in DbContext.GetType().GetProperties())
            {
                if (!property.PropertyType.IsGenericType)
                    continue;
                if (property.PropertyType.GetGenericTypeDefinition() != typeof(DbSet<>))
                    continue;
                types.Add(property.PropertyType.GetGenericArguments()[0]);
            }
            EntityTypes = types.ToArray();
        }

        public IEntityQueryable<TEntity> GetContext<TEntity>() where TEntity : EntityBase, new()
        {
            if (!EntityTypes.Contains(typeof(TEntity)))
                throw new ArgumentException("TEntity不属于该Context。");
            return (IEntityQueryable<TEntity>)Activator.CreateInstance(typeof(EntityQueryable<>).MakeGenericType(typeof(TEntity)), DbContext);
        }

        public Type[] EntityTypes { get; private set; }

        public ICacheEntityQueryable<TEntity> GetCacheContext<TEntity>() where TEntity : CacheEntityBase, new()
        {
            if (!EntityTypes.Contains(typeof(TEntity)))
                throw new ArgumentException("TEntity不属于该Context。");
            return new CacheEntityQueryable<TEntity>(DbContext);
        }
        
        public object GetContext(Type entityType)
        {
            if (!EntityTypes.Contains(entityType))
                throw new ArgumentException("TEntity不属于该Context。");
            return Activator.CreateInstance(typeof(EntityQueryable<>).MakeGenericType(entityType), DbContext);
        }
    }
}
