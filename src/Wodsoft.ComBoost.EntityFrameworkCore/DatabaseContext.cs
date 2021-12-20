using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity;
using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Collections.ObjectModel;
using System.Transactions;

namespace Wodsoft.ComBoost.Data.Entity
{
    public abstract class DatabaseContext : IDatabaseContext
    {
        private static ConcurrentDictionary<Type, IEnumerable<Type>> _CachedSupportTypes;
        private Dictionary<Type, object> _CachedEntityContext;

        static DatabaseContext()
        {
            _CachedSupportTypes = new ConcurrentDictionary<Type, IEnumerable<Type>>();
        }

        public DbContext InnerContext { get; private set; }

        public abstract IEnumerable<Type> SupportTypes { get; }

        public bool TrackEntity { get; set; }

        public DatabaseContext(DbContext context)
        {
            TrackEntity = false;
            _CachedEntityContext = new Dictionary<Type, object>();
            InnerContext = context;
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

        public IDatabaseTransaction CreateTransaction()
        {
            return new DatabaseTransaction(InnerContext.Database.BeginTransaction());
        }
    }

    public class DatabaseContext<TDbContext> : DatabaseContext
        where TDbContext : DbContext
    {
        private static ReadOnlyCollection<Type> _supportTypes;

        static DatabaseContext()
        {
            _supportTypes = new ReadOnlyCollection<Type>(typeof(TDbContext).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(t => t.CanRead && t.CanWrite && t.PropertyType.IsConstructedGenericType && t.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
                    .Select(t => t.PropertyType.GetGenericArguments()[0]).ToArray());
        }

        public DatabaseContext(TDbContext context) : base(context)
        {

        }

        public override IEnumerable<Type> SupportTypes => _supportTypes;

        public static IEntityContext<TEntity> GetEntityContextDelegate<TEntity>(IServiceProvider serviceProvider)
            where TEntity : class, IEntity, new()
        {
            var dataContext = serviceProvider.GetService<DatabaseContext<TDbContext>>();
            return dataContext.GetContext<TEntity>();
        }
    }
}
