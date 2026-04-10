using StackExchange.Redis;
using System;
using System.Collections.Generic;
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
        private bool _disposedValue;
        private bool _locked;
        private CancellationTokenSource _lockerCTS;

        public RedisSemaphore(IConnectionMultiplexer connection, IDatabase database, string key)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _database = database ?? throw new ArgumentNullException(nameof(database));
            _key = key ?? throw new ArgumentNullException(nameof(key));
        }

        public async Task EnterAsync(CancellationToken cancellationToken = default)
        {
            if (_disposedValue)
                throw new ObjectDisposedException(nameof(RedisSemaphore));
            if (_locked)
                throw new InvalidOperationException("Already entered.");
            if (await _database.LockTakeAsync(_key, Environment.MachineName, TimeSpan.FromSeconds(10)).ConfigureAwait(false))
            {
                _locked = true;
                _lockerCTS = new CancellationTokenSource();
                _ = KeepLock(_lockerCTS.Token);
                return;
            }
#if NETSTANDARD2_0
            TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();
#else
            TaskCompletionSource taskCompletionSource = new TaskCompletionSource();
#endif
            var subscriber = _connection.GetSubscriber();
            var subscribeChannel = new RedisChannel(_key + "_Notify", RedisChannel.PatternMode.Literal);
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
            if (cancellationToken.CanBeCanceled)
                cancellationToken.Register(() =>
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
                while (true)
                {
                    await Task.WhenAny(Task.Delay(5000), Volatile.Read(ref taskCompletionSource).Task).ConfigureAwait(false);
                    if (await _database.LockTakeAsync(_key, Environment.MachineName, TimeSpan.FromSeconds(10)).ConfigureAwait(false))
                    {
                        _locked = true;
                        _lockerCTS = new CancellationTokenSource();
                        _ = KeepLock(_lockerCTS.Token);
                        break;
                    }
                }
            }
            finally
            {
                await subscriber.UnsubscribeAsync(subscribeChannel).ConfigureAwait(false);
            }
        }

        public async Task<bool> EnterAsync(int timeout, CancellationToken cancellationToken = default)
        {
            if (_disposedValue)
                throw new ObjectDisposedException(nameof(RedisSemaphore));
            if (_locked)
                throw new InvalidOperationException("Already entered.");
            if (await _database.LockTakeAsync(_key, Environment.MachineName, TimeSpan.FromSeconds(10)).ConfigureAwait(false))
            {
                _locked = true;
                _lockerCTS = new CancellationTokenSource();
                _ = KeepLock(_lockerCTS.Token);
                return true;
            }
            if (timeout == 0)
                return false;
#if NETSTANDARD2_0
            TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();
#else
            TaskCompletionSource taskCompletionSource = new TaskCompletionSource();
#endif
            var subscriber = _connection.GetSubscriber();
            var subscribeChannel = new RedisChannel(_key + "_Notify", RedisChannel.PatternMode.Literal);
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
                cts.CancelAfter(timeout);
            }
            else
                cts = new CancellationTokenSource(timeout);
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
                while (true)
                {
                    try
                    {
                        await Task.WhenAny(Task.Delay(5000), Volatile.Read(ref taskCompletionSource).Task).ConfigureAwait(false);
                    }
                    catch
                    {
                        return false;
                    }
                    if (await _database.LockTakeAsync(_key, Environment.MachineName, TimeSpan.FromSeconds(10)).ConfigureAwait(false))
                    {
                        _locked = true;
                        _lockerCTS = new CancellationTokenSource();
                        _ = KeepLock(_lockerCTS.Token);
                        return true;
                    }
                }
            }
            finally
            {
                await subscriber.UnsubscribeAsync(subscribeChannel).ConfigureAwait(false);
            }
        }

        public async Task ExitAsync()
        {
            if (_disposedValue)
                throw new ObjectDisposedException(nameof(RedisSemaphore));
            if (!_locked)
                throw new InvalidOperationException("ot entered.");
            _locked = false;
            _lockerCTS.Cancel();
            _lockerCTS.Dispose();
            _lockerCTS = null;
            await _database.LockReleaseAsync(_key, Environment.MachineName).ConfigureAwait(false);
            var subscriber = _connection.GetSubscriber();
            var subscribeChannel = new RedisChannel(_key + "_Notify", RedisChannel.PatternMode.Literal);
            await subscriber.PublishAsync(subscribeChannel, "release").ConfigureAwait(false);
        }

        public async Task<bool> TryEnterAsync()
        {
            if (_disposedValue)
                throw new ObjectDisposedException(nameof(RedisSemaphore));
            if (_locked)
                throw new InvalidOperationException("Already entered.");
            if (await _database.LockTakeAsync(_key, Environment.MachineName, TimeSpan.FromSeconds(10)).ConfigureAwait(false))
            {
                _locked = true;
                _lockerCTS = new CancellationTokenSource();
                _ = KeepLock(_lockerCTS.Token);
                return true;
            }
            return false;
        }

        private async Task KeepLock(CancellationToken cancellationToken)
        {
            while (true)
            {
                await Task.Delay(5000).ConfigureAwait(false);
                if (cancellationToken.IsCancellationRequested)
                    return;
                await _database.KeyExpireAsync(_key, TimeSpan.FromSeconds(10)).ConfigureAwait(false);
            }
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
    }
}
