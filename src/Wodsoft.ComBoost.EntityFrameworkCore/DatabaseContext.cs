using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity;
using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace Wodsoft.ComBoost.Data.Entity
{
    public class DatabaseContext : IDatabaseContext
    {
        private static ConcurrentDictionary<Type, IEnumerable<Type>> _CachedSupportTypes;
        private ComboostEntityStateListener _StateListener;
        private Dictionary<Type, object> _CachedEntityContext;

        static DatabaseContext()
        {
            _CachedSupportTypes = new ConcurrentDictionary<Type, IEnumerable<Type>>();
        }

        public DbContext InnerContext { get; private set; }

        public IEnumerable<Type> SupportTypes { get; private set; }

        public DatabaseContext(DbContext context)
        {
            _CachedEntityContext = new Dictionary<Type, object>();
            _StateListener = (ComboostEntityStateListener)context.GetInfrastructure().GetServices<IEntityStateListener>().FirstOrDefault(t => t is ComboostEntityStateListener);
            if (_StateListener == null)
                throw new ArgumentException("该EntityFramework数据库上下文未配置使用ComBoostOptionExtension扩展。");
            _StateListener.EntityInit += _StateListener_EntityInit;
            InnerContext = context;
            SupportTypes = _CachedSupportTypes.GetOrAdd(context.GetType(), type =>
            {
                var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(t => t.CanRead && t.CanWrite && t.PropertyType.IsConstructedGenericType && t.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
                    .Select(t => t.PropertyType.GetGenericArguments()[0]).ToList();
                return new System.Collections.ObjectModel.ReadOnlyCollection<Type>(properties);
            });
        }

        private void _StateListener_EntityInit(InternalEntityEntry obj)
        {
            IEntity entity = obj.Entity as IEntity;
            if (entity == null)
                return;
            object context;
            if (_CachedEntityContext.ContainsKey(obj.EntityType.ClrType))
                context = _CachedEntityContext[obj.EntityType.ClrType];
            else
            {
                context = this.GetDynamicContext(obj.EntityType.ClrType);
            }
            entity.EntityContext = (IEntityQueryContext<IEntity>)context;
        }

        public Task<int> SaveAsync()
        {
            return InnerContext.SaveChangesAsync();
        }

        public IEntityContext<T> GetContext<T>() where T : class, IEntity, new()
        {
            if (_CachedEntityContext.ContainsKey(typeof(T)))
                return (EntityContext<T>)_CachedEntityContext[typeof(T)];
            var context = new EntityContext<T>(this, InnerContext.Set<T>());
            _CachedEntityContext.Add(typeof(T), context);
            return context;
        }
    }
}
