using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.StackExchangeRedis
{
    public class RedisSemaphore : ISemaphore, IAsyncDisposable
    {
        private readonly IConnectionMultiplexer _connection;
        private readonly IDatabase _database;
        private readonly string _key;
        private readonly int _lockTimeout;
        private bool _disposedValue;
        private bool _locked;
        private Task _keepLockTask;
        private CancellationTokenSource _lockerCTS;

        public RedisSemaphore(IConnectionMultiplexer connection, IDatabase database, string key, int lockTimeout = 10000)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _database = database ?? throw new ArgumentNullException(nameof(database));
            _key = key ?? throw new ArgumentNullException(nameof(key));
            if (lockTimeout <= 0)
                throw new ArgumentOutOfRangeException("Lock timeout can't less or equal than zero.");
            _lockTimeout = lockTimeout;
        }

        public async Task EnterAsync(CancellationToken cancellationToken = default)
        {
            if (_disposedValue)
                throw new ObjectDisposedException(nameof(RedisSemaphore));
            if (_locked)
                throw new InvalidOperationException("Already entered.");
            await TakeLockAsync(_connection, _database, _key, -1, cancellationToken).ConfigureAwait(false);
            _locked = true;
            _lockerCTS = new CancellationTokenSource();
            _keepLockTask = KeepLockAsync(_connection, _database, _key, _lockTimeout, _lockerCTS.Token);
        }

        public async Task<bool> EnterAsync(int timeout, CancellationToken cancellationToken = default)
        {
            if (_disposedValue)
                throw new ObjectDisposedException(nameof(RedisSemaphore));
            if (_locked)
                throw new InvalidOperationException("Already entered.");
            if (!await TakeLockAsync(_connection, _database, _key, timeout, cancellationToken).ConfigureAwait(false))
                return false;
            _locked = true;
            _lockerCTS = new CancellationTokenSource();
            _keepLockTask = KeepLockAsync(_connection, _database, _key, _lockTimeout, _lockerCTS.Token);
            return true;
        }

        public Task ExitAsync()
        {
            if (_disposedValue)
                throw new ObjectDisposedException(nameof(RedisSemaphore));
            if (!_locked)
                throw new InvalidOperationException("ot entered.");
            _locked = false;
            _lockerCTS.Cancel();
            _lockerCTS.Dispose();
            _lockerCTS = null;
            return _keepLockTask;
        }

        public async Task<bool> TryEnterAsync()
        {
            if (_disposedValue)
                throw new ObjectDisposedException(nameof(RedisSemaphore));
            if (_locked)
                throw new InvalidOperationException("Already entered.");
            if (await TakeLockAsync(_connection, _database, _key, 0, default).ConfigureAwait(false))
            {
                _locked = true;
                _lockerCTS = new CancellationTokenSource();
                _keepLockTask = KeepLockAsync(_connection, _database, _key, _lockTimeout, _lockerCTS.Token);
                return true;
            }
            return false;
        }

        public async ValueTask DisposeAsync()
        {
            if (!_disposedValue)
            {
                if (_locked)
                {
                    await ExitAsync().ConfigureAwait(false);
                }
                _disposedValue = true;
            }
        }

        internal static async Task<bool> TakeLockAsync(IConnectionMultiplexer connection, IDatabase database, string key, int timeout, CancellationToken cancellationToken)
        {
            var lockTimeSpan = TimeSpan.FromSeconds(timeout);
            if (await database.LockTakeAsync(key, Environment.MachineName, lockTimeSpan).ConfigureAwait(false))
                return true;
            if (timeout == 0)
                return false;
#if NETSTANDARD2_0
            TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();
#else
            TaskCompletionSource taskCompletionSource = new TaskCompletionSource();
#endif
            var subscriber = connection.GetSubscriber();
            var subscribeChannel = new RedisChannel(key + "_Notify", RedisChannel.PatternMode.Literal);
            await subscriber.SubscribeAsync(subscribeChannel, (channel, value) =>
            {
#if NETSTANDARD2_0
                var tcs = Interlocked.Exchange(ref taskCompletionSource, new TaskCompletionSource<bool>());
                tcs.TrySetResult(true);
#else
                var tcs = Interlocked.Exchange(ref taskCompletionSource, new TaskCompletionSource());
                tcs.TrySetResult();
#endif
            }).ConfigureAwait(false);
            CancellationTokenSource cts;
            if (cancellationToken.CanBeCanceled)
            {
                cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                if (timeout > 0)
                    cts.CancelAfter(timeout);
            }
            else
            {
                if (timeout > 0)
                    cts = new CancellationTokenSource(timeout);
                else
                    cts = new CancellationTokenSource();
            }
            cts.Token.Register(() =>
            {
#if NETSTANDARD2_0
                var tcs = Interlocked.Exchange(ref taskCompletionSource, new TaskCompletionSource<bool>());
#else
                var tcs = Interlocked.Exchange(ref taskCompletionSource, new TaskCompletionSource());
#endif
                tcs.SetCanceled();
            });
            try
            {
                var waitTimeout = timeout / 2;
                while (true)
                {
                    try
                    {
                        await Task.WhenAny(Task.Delay(waitTimeout), Volatile.Read(ref taskCompletionSource).Task).ConfigureAwait(false);
                    }
                    catch
                    {
                        if (cancellationToken.IsCancellationRequested)
                            throw;
                        return false;
                    }
                    if (await database.LockTakeAsync(key, Environment.MachineName, lockTimeSpan).ConfigureAwait(false))
                        return true;
                }
            }
            finally
            {
                await subscriber.UnsubscribeAsync(subscribeChannel).ConfigureAwait(false);
                cts.Dispose();
            }
        }

        internal static async Task KeepLockAsync(IConnectionMultiplexer connection, IDatabase database, string key, int timeout, CancellationToken cancellationToken)
        {
            var halfTime = timeout / 2;
            var timeSpan = TimeSpan.FromMilliseconds(timeout);
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    await Task.Delay(halfTime, cancellationToken).ConfigureAwait(false);
                    if (cancellationToken.IsCancellationRequested)
                        return;
                    await database.KeyExpireAsync(key, timeSpan).ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException)
            {
                return;
            }
            finally
            {
                await database.LockReleaseAsync(key, Environment.MachineName).ConfigureAwait(false);
                var subscriber = connection.GetSubscriber();
                var subscribeChannel = new RedisChannel(key + "_Notify", RedisChannel.PatternMode.Literal);
                await subscriber.PublishAsync(subscribeChannel, "release").ConfigureAwait(false);
            }
        }
    }

    public class RedisSemaphoreScope : IDisposable
#if !NETSTANDARD2_0
        , IAsyncDisposable
#endif
    {
        private readonly CancellationTokenSource _cts;
        private readonly Task _keepLockTask;
        private int _disposed;

        public RedisSemaphoreScope(Task keepLockTask, CancellationTokenSource cancellationTokenSource)
        {
            _cts = cancellationTokenSource;
            _keepLockTask = keepLockTask;
        }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref _disposed, 1) == 1)
                return;
            _cts.Cancel(false);
            _cts.Dispose();
        }

#if !NETSTANDARD2_0
        public ValueTask DisposeAsync()
        {
            if (Interlocked.Exchange(ref _disposed, 1) == 1)
                return default;
            _cts.Cancel();
            _cts.Dispose();
            async ValueTask waitToCompleted(Task task) => await task.ConfigureAwait(false);
            return waitToCompleted(_keepLockTask);
        }
#endif
    }
}
