using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
            return new RedisSemaphore(_connection, _database, _option.Prefix + name, _option.LockTimeout);
        }

#if NETSTANDARD2_0
        public async Task<IDisposable> EnterScopeAsync(string name, CancellationToken cancellationToken = default)
#else
        public async Task<IAsyncDisposable> EnterScopeAsync(string name, CancellationToken cancellationToken = default)
#endif
        {
            await RedisSemaphore.TakeLockAsync(_connection, _database, name, -1, cancellationToken);
            CancellationTokenSource cts = new CancellationTokenSource();
            var task = RedisSemaphore.KeepLockAsync(_connection, _database, name, _option.LockTimeout, cts.Token);
            return new RedisSemaphoreScope(task, cts);
        }
    }
}
