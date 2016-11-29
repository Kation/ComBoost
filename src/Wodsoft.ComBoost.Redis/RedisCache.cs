using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Redis
{
    public class RedisCache : ICache
    {
        private IDatabase _Database;
        private ISerializerProvider _SerializerProvider;
        public RedisCache(IDatabase database, ISerializerProvider serializerProvider)
        {
            if (database == null)
                throw new ArgumentNullException(nameof(database));
            if (serializerProvider == null)
                throw new ArgumentNullException(nameof(serializerProvider));
            _Database = database;
            _SerializerProvider = serializerProvider;
        }

        public async Task<object> GetAsync(string name, Type valueType)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (valueType == null)
                throw new ArgumentNullException(nameof(valueType));
            var data = await _Database.KeyDumpAsync(name);
            if (data == null)
                return null;
            var seralizer = _SerializerProvider.GetSerializer(valueType);
            var stream = new MemoryStream(data);
            return seralizer.Deserialize(stream);
        }

        public Task<bool> DeleteAsync(string name)
        {
            return _Database.KeyDeleteAsync(name);
        }

        public async Task SetAsync(string name, object value, TimeSpan? expireTime)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            var seralizer = _SerializerProvider.GetSerializer(value.GetType());
            var stream = new MemoryStream();
            seralizer.Serialize(stream, value);
            var data = stream.ToArray();
            await _Database.KeyRestoreAsync(name, data, expireTime);
        }
    }
}
