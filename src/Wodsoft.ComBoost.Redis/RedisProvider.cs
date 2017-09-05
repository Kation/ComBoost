using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Redis
{
    public class RedisProvider : ICacheProvider, ISemaphoreProvider
    {
        private ISerializerProvider _SerializerProvider;

        public RedisProvider(string connectionAddress, ISerializerProvider serializerProvider, int database = -1)
        {
            if (connectionAddress == null)
                throw new ArgumentNullException(nameof(connectionAddress));
            if (serializerProvider == null)
                throw new ArgumentNullException(nameof(serializerProvider));
            Connection = ConnectionMultiplexer.Connect(connectionAddress);
            _SerializerProvider = serializerProvider;
            LockerTimeout = TimeSpan.FromMinutes(15);
            Datbase = database;
        }

        public TimeSpan LockerTimeout { get; set; }

        public ConnectionMultiplexer Connection { get; private set; }

        public int Database { get; private set; }

        public ICache GetCache()
        {
            return new RedisCache(Connection.GetDatabase(Database), _SerializerProvider);
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
