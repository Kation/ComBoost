using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    public class MemoryCacheProvider : ICacheProvider
    {
        public MemoryCacheProvider(ISerializerProvider serializerProvider)
        {
            if (serializerProvider == null)
                throw new ArgumentNullException(nameof(serializerProvider));
            SerializerProvider = serializerProvider;
        }

        public ISerializerProvider SerializerProvider { get; private set; }

        private MemoryCache _Cache;
        public ICache GetCache()
        {
            if (_Cache == null)
                _Cache = new MemoryCache(SerializerProvider);
            return _Cache;
        }
    }

    public class MemoryCache : ICache
    {
        private ConcurrentDictionary<string, CacheEntry> _Entries;

        public MemoryCache(ISerializerProvider serializerProvider)
        {
            if (serializerProvider == null)
                throw new ArgumentNullException(nameof(serializerProvider));
            SerializerProvider = serializerProvider;
            _Entries = new ConcurrentDictionary<string, MemoryCache.CacheEntry>();
        }

        public ISerializerProvider SerializerProvider { get; private set; }

        public Task<bool> DeleteAsync(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            CacheEntry entry;
            return Task.FromResult(_Entries.TryRemove(name, out entry));
        }

        private static readonly Task<object> _NullTask = Task.FromResult<object>(null);
        public Task<object> GetAsync(string name, Type valueType)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (valueType == null)
                throw new ArgumentNullException(nameof(valueType));
            CacheEntry entry;
            if (!_Entries.TryGetValue(name, out entry))
                return _NullTask;
            if (DateTime.Now < entry.ExpiredDate)
            {
                _Entries.TryRemove(name, out entry);
                return _NullTask;
            }
            var serializer = SerializerProvider.GetSerializer(valueType);
            return Task.FromResult(serializer.Deserialize(new MemoryStream(entry.Value)));
        }

        public Task SetAsync(string name, object value, TimeSpan? expireTime)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            CacheEntry entry = new CacheEntry();
            var serializer= SerializerProvider.GetSerializer(value.GetType());
            MemoryStream stream = new MemoryStream();
            serializer.Serialize(stream, value);
            entry.Value = stream.ToArray();
            if (expireTime.HasValue)
                entry.ExpiredDate = DateTime.Now.Add(expireTime.Value);
            _Entries.AddOrUpdate(name, n => entry, (n, o) => entry);
            return Task.CompletedTask;
        }

        private class CacheEntry
        {
            public byte[] Value { get; set; }

            public DateTime? ExpiredDate { get; set; }
        }
    }
}
