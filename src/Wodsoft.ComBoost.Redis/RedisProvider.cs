using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Redis
{
    public class RedisProvider : ICacheProvider, ISemaphoreProvider
    {
        private ConnectionMultiplexer _Conn;
        private ISerializerProvider _SerializerProvider;

        public RedisProvider(string connectionAddress, ISerializerProvider serializerProvider)
        {
            if (connectionAddress == null)
                throw new ArgumentNullException(nameof(connectionAddress));
            if (serializerProvider == null)
                throw new ArgumentNullException(nameof(serializerProvider));
            _Conn = ConnectionMultiplexer.Connect(connectionAddress);
            _SerializerProvider = serializerProvider;
            LockerTimeout = TimeSpan.FromMinutes(15);
        }

        public TimeSpan LockerTimeout { get; set; }

        public ICache GetCache()
        {
            return new RedisCache(_Conn.GetDatabase(), _SerializerProvider);
        }

        public ICache GetCache(string name)
        {
            throw new NotSupportedException();
        }

        public ISemaphore GetSemaphore(string name)
        {
            return new RedisSemaphore(_Conn.GetDatabase(), name, LockerTimeout);
        }
    }
}
