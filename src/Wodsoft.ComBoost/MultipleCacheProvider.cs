using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    public class MultipleCacheProvider : ICacheProvider
    {
        private Dictionary<string, Type> _CacheType;

        public MultipleCacheProvider(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            _CacheType = new Dictionary<string, Type>();
        }

        public IServiceProvider ServiceProvider { get; private set; }

        public virtual void AddProvider<T>(string name)
            where T : class, ICacheProvider
        {
            if (_CacheType.ContainsKey(name))
                throw new ArgumentException("缓存提供器名称已存在。");
            _CacheType.Add(name, typeof(T));
        }

        public virtual void RemoveProvider(string name)
        {
            _CacheType.Remove(name);
        }

        public virtual ICache GetCache()
        {
            throw new NotSupportedException("找不到缓存提供器。");
        }

        public virtual ICache GetCache(string name)
        {
            Type type;
            if (!_CacheType.TryGetValue(name, out type))
                throw new InvalidOperationException("找不到缓存提供器。");
            ICacheProvider provider = (ICacheProvider)ServiceProvider.GetRequiredService(type);
            return provider.GetCache();
        }
    }

    public class MultipleCacheProvider<T> : MultipleCacheProvider, ICacheProvider
        where T : class, ICacheProvider
    {
        public MultipleCacheProvider(IServiceProvider serviceProvider) : base(serviceProvider)
        {

        }

        public override ICache GetCache()
        {
            ICacheProvider provider = ServiceProvider.GetRequiredService<T>();
            return provider.GetCache();
        }
    }
}
