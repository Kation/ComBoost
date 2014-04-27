using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using Wodsoft.Net.Service;

namespace System.ServiceModel
{
    public class EntityServiceChannelBuilder
    {
        public CacheEntityContextBuilder Builder;
        private EntityServiceFormatter DataFormatter;

        public EntityServiceChannelBuilder(CacheEntityContextBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException("builder");
            Builder = builder;
            DataFormatter = new EntityServiceFormatter(builder);
        }

        public ServiceChannel[] GetChannels()
        {
            List<ServiceChannel> list = new List<ServiceChannel>();
            foreach (var type in Builder.EntityTypes)
            {
                Type instance = typeof(CacheEntityQueryable<>).MakeGenericType(new Type[] { type });
                Type contract = typeof(ICacheEntityQueryable<>).MakeGenericType(new Type[] { type });
                ServiceProvider provider = new ServiceProvider(instance, contract);
                ServiceChannel channel = new ServiceChannel("Comboost_EntityChannel_" + type.Name, provider, DataFormatter);
                list.Add(channel);
            }
            return list.ToArray();
        }

        public void SetUnity(ServiceUnity unity)
        {
            unity.RegisterType<DbContext>(Builder.DbContext);
        }

        public void SetService(ServiceHost host)
        {
            foreach (var channel in GetChannels())
                host.RegisterChannel(channel);
            SetUnity(host.Unity);
        }
    }
}
