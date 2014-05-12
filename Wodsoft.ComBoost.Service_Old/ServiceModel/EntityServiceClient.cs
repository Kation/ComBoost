using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Net;
using Wodsoft.Net.Service;

namespace System.ServiceModel
{
    public class EntityServiceClient : ServiceClient, ICacheEntityContextBuilder
    {
        private DbContext LocalContext;
        private Dictionary<Type, ServiceChannel> Channels;
        private Dictionary<Type, object> Factories, Instaces;

        public EntityServiceClient(DbContext localContext, IPEndPoint endPoint)
            : base(endPoint)
        {
            DataFormatter = new EntityServiceFormatter(this);

            if (LocalContext == null)
                throw new ArgumentNullException("localContext");
            LocalContext = localContext;
                        
            List<Type> types = new List<Type>();
            foreach (var property in LocalContext.GetType().GetProperties())
            {
                if (!property.PropertyType.IsGenericType)
                    continue;
                if (property.PropertyType.GetGenericTypeDefinition() != typeof(DbSet<>))
                    continue;
                types.Add(property.PropertyType.GetGenericArguments()[0]);
            }
            EntityTypes = types.ToArray();

            Channels = new Dictionary<Type, ServiceChannel>();
            foreach (var type in EntityTypes)
            {
                Type contract = typeof(ICacheEntityQueryable<>).MakeGenericType(new Type[] { type });
                ServiceProvider provider = new ServiceProvider(null, contract);
                ServiceChannel channel = new ServiceChannel("Comboost_EntityChannel_" + type.Name, provider, DataFormatter);
                Channels.Add(type, channel);
            }

            Factories = new Dictionary<Type, object>();
            Instaces = new Dictionary<Type, object>();
        }

        public Type[] EntityTypes { get; private set; }

        public IEntityQueryable<TEntity> GetContext<TEntity>() where TEntity : EntityBase, new()
        {
            if (!EntityTypes.Contains(typeof(TEntity)))
                throw new ArgumentException("TEntity不属于该Context。");
            return GetContext(typeof(TEntity)) as IEntityQueryable<TEntity>;
        }

        public object GetContext(Type entityType)
        {
            if (!EntityTypes.Contains(entityType))
                throw new ArgumentException("TEntity不属于该Context。");
            object factory;
            if (!Factories.ContainsKey(entityType))
                Factories.Add(entityType, GetType().GetMethod("GetChannelFactory").MakeGenericMethod(typeof(ICacheEntityQueryable<>).MakeGenericType(entityType)).Invoke(this, new object[] { Channels[entityType] }));
            factory = Factories[entityType];
            object context;
            if (!Instaces.ContainsKey(entityType))
            {
                bool exist = (bool)factory.GetType().GetProperty("Exist").GetValue(factory, null);
                if (!exist)
                    return null;
                object remoteContext =factory.GetType().GetMethod("GetChannel").Invoke(factory, null);
                object queryable = Activator.CreateInstance(typeof(CacheLocalEntityQueryable<>).MakeGenericType(entityType), new object[] { remoteContext, LocalContext });
                Instaces.Add(entityType, queryable);
            }
            context = Instaces[entityType];
            return context;
        }

        public ICacheEntityQueryable<TEntity> GetCacheContext<TEntity>() where TEntity : CacheEntityBase, new()
        {
            if (!EntityTypes.Contains(typeof(TEntity)))
                throw new ArgumentException("TEntity不属于该Context。");
            return GetContext(typeof(TEntity)) as ICacheEntityQueryable<TEntity>;
        }
    }
}
