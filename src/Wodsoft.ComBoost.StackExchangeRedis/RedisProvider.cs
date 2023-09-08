using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost.StackExchangeRedis
{
    public class RedisProvider : ISemaphoreProvider, IDisposable
    {
        private RedisOptions _option;
        private IConnectionMultiplexer _connection;
        private IDatabase _database;

        public RedisProvider(IOptions<RedisOptions> options)
        {
            _option = options?.Value ?? throw new ArgumentNullException(nameof(options));
            if (_option.Configuration == null)
                throw new ArgumentException();
            _connection = ConnectionMultiplexer.Connect(_option.Configuration);
            _database = _connection.GetDatabase();
        }

        private bool _disposed;
        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                _connection.Close();
            }
        }

        public ISemaphore GetSemaphore(string name)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(RedisProvider));
            return new RedisSemaphore(_connection, _database, _option.Prefix + name);
        }
    }
}
