using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.AspNetCore
{
    public class SessionCacheProvider : ICacheProvider
    {
        public SessionCacheProvider(IHttpContextAccessor httpContextAccessor, ISerializerProvider serializerProvider)
        {
            if (httpContextAccessor == null)
                throw new ArgumentNullException(nameof(httpContextAccessor));
            HttpContext = httpContextAccessor.HttpContext;
            SerializerProvider = serializerProvider ?? throw new ArgumentNullException(nameof(serializerProvider));
        }

        public HttpContext HttpContext { get; private set; }

        public ISerializerProvider SerializerProvider { get; private set; }

        private SessionCache _Cache;

        /// <inheritdoc/>
        public ICache GetCache()
        {
            if (_Cache == null)
                _Cache = new SessionCache("__ComBoost_Cache_", HttpContext.Session, SerializerProvider);
            return _Cache;
        }

        public ICache GetCache(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            return new SessionCache("__ComBoost_Cache_" + name + "_", HttpContext.Session, SerializerProvider);
        }
    }

    public class SessionCache : ICache
    {
        public SessionCache(string prefix, ISession session, ISerializerProvider serializerProvider)
        {
            Prefix = prefix ?? throw new ArgumentNullException(nameof(prefix));
            Session = session ?? throw new ArgumentNullException(nameof(session));
            SerializerProvider = serializerProvider ?? throw new ArgumentNullException(nameof(serializerProvider));
        }

        public string Prefix { get; private set; }

        public ISession Session { get; private set; }

        public ISerializerProvider SerializerProvider { get; private set; }

        public Task ClearAsync()
        {
            var keys = Session.Keys.Where(t => t.StartsWith(Prefix)).ToArray();
            foreach (var key in keys)
                Session.Remove(key);
            return Task.CompletedTask;
        }

        public Task<string[]> GetKeysAsync()
        {
            return Task.FromResult(Session.Keys.Where(t => t.StartsWith(Prefix)).ToArray());
        }

        public Task<bool> DeleteAsync(string name)
        {
            Session.Remove(Prefix + name);
            return Task.FromResult(true);
        }

        public Task<object> GetAsync(string name, Type valueType)
        {
            byte[] data;
            if (!Session.TryGetValue(Prefix + name, out data))
                return Task.FromResult<object>(null);
            var stream = new MemoryStream(data);
            var type = typeof(CacheItem<>).MakeGenericType(valueType);
            dynamic item = SerializerProvider.GetSerializer(type).Deserialize(stream);
            DateTime? expiredDate = item.ExpiredDate;
            if (expiredDate < DateTime.Now)
                return Task.FromResult<object>(null);
            return Task.FromResult<object>(item.Value);
        }

        public Task SetAsync(string name, object value, TimeSpan? expireTime)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            var type = typeof(CacheItem<>).MakeGenericType(value.GetType());
            dynamic item = Activator.CreateInstance(type);
            type.GetProperty("Value").SetValue(item, value);
            if (expireTime.HasValue)
                item.ExpiredDate = DateTime.Now.Add(expireTime.Value);
            var stream = new MemoryStream();
            SerializerProvider.GetSerializer(type).Serialize(stream, item);
            var data = stream.ToArray();
            Session.Set(Prefix + name, data);
            return Task.CompletedTask;
        }

        private class CacheItem<T>
        {
            public T Value { get; set; }

            public DateTime? ExpiredDate { get; set; }
        }
    }
}
