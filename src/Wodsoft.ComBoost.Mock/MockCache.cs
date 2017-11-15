using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Mock
{
    public class MockCache : ICache
    {
        private ConcurrentDictionary<string, MockCacheEntry> _Cache;

        public MockCache()
        {
            _Cache = new ConcurrentDictionary<string, MockCacheEntry>();
        }

        public Task ClearAsync()
        {
            _Cache.Clear();
            return Task.CompletedTask;
        }

        public Task<bool> DeleteAsync(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            MockCacheEntry entry;
            return Task.FromResult(_Cache.TryGetValue(name, out entry));
        }

        public Task<object> GetAsync(string name, Type valueType)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            MockCacheEntry entry;
            if (_Cache.TryGetValue(name, out entry))
                if (!valueType.IsAssignableFrom(entry.ValueType))
                    throw new InvalidCastException("类型不正确。");
            if (entry.ExpiredDate.HasValue && DateTime.Now > entry.ExpiredDate)
            {
                _Cache.TryRemove(name, out entry);
                return Task.FromResult<object>(null);
            }
            return Task.FromResult(entry.Value);
        }

        public Task<string[]> GetKeysAsync()
        {
            throw new NotImplementedException();
        }

        public Task SetAsync(string name, object value, TimeSpan? expireTime)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            _Cache.TryAdd(name, new MockCacheEntry
            {
                Value = value,
                ValueType = value.GetType(),
                ExpiredDate = expireTime.HasValue ? (DateTime?)DateTime.Now.Add(expireTime.Value) : null
            });
            return Task.CompletedTask;
        }

        private class MockCacheEntry
        {
            public Type ValueType { get; set; }

            public object Value { get; set; }

            public DateTime? ExpiredDate { get; set; }
        }
    }
}
