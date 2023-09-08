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
        private IDatabase _database;
        private string _key, _value;
        private bool _entered;
        private Task _keepAliveTask;
        private CancellationTokenSource _keepAliveCTS;

        public RedisSemaphore(IConnectionMultiplexer connection, IDatabase database, string key)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _database = database ?? throw new ArgumentNullException(nameof(database));
            _key = key ?? throw new ArgumentNullException(nameof(key));
            _entered = false;
            _value = Guid.NewGuid().ToString();
        }

        public async Task EnterAsync(CancellationToken cancellationToken)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(RedisSemaphore));
            if (_entered)
                throw new InvalidOperationException("Already entered.");
#if NETSTANDARD2_0
            TaskCompletionSource<bool> taskCompletionSource = null;
#else
            TaskCompletionSource taskCompletionSource = null;
#endif
            Task notifyTask = Task.CompletedTask;
            var subscriber = _connection.GetSubscriber();
            await subscriber.SubscribeAsync(_key + "_Notify", (channel, value) =>
            {
#if NETSTANDARD2_0
                taskCompletionSource.SetResult(true);
#else
                taskCompletionSource.SetResult();
#endif
            });
            cancellationToken.Register(() => taskCompletionSource.SetCanceled());
            try
            {
                while (true)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var timeout = Task.Delay(5000);
                    await Task.WhenAny(timeout, notifyTask);
                    cancellationToken.ThrowIfCancellationRequested();
                    if (await _database.LockTakeAsync(_key, _value, TimeSpan.FromSeconds(30)))
                    {
                        _entered = true;
                        _keepAliveTask = Task.Run(KeepAlive);
                        return;
                    }
#if NETSTANDARD2_0
                    taskCompletionSource = new TaskCompletionSource<bool>();
#else
                    taskCompletionSource = new TaskCompletionSource();
#endif
                    notifyTask = taskCompletionSource.Task;
                }
            }
            finally
            {
                await subscriber.UnsubscribeAsync(_key + "_Notify");
            }
        }

        public async Task<bool> EnterAsync(int timeout)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(RedisSemaphore));
            if (_entered)
                throw new InvalidOperationException("Already entered.");
#if NETSTANDARD2_0
            TaskCompletionSource<bool> taskCompletionSource = null;
#else
            TaskCompletionSource taskCompletionSource = null;
#endif
            Task notifyTask = Task.CompletedTask;
            var subscriber = _connection.GetSubscriber();
            await subscriber.SubscribeAsync(_key + "_Notify", (channel, value) =>
            {
#if NETSTANDARD2_0
                taskCompletionSource.SetResult(true);
#else
                taskCompletionSource.SetResult();
#endif
            });
            try
            {
                var timeoutTask = Task.Delay(timeout);
                while (true)
                {
                    await Task.WhenAny(notifyTask, timeoutTask);
                    if (timeoutTask.IsCompleted)
                        return false;
                    if (await _database.LockTakeAsync(_key, _value, TimeSpan.FromSeconds(30)))
                    {
                        _entered = true;
                        _keepAliveTask = Task.Run(KeepAlive);
                        return true;
                    }
#if NETSTANDARD2_0
                    taskCompletionSource = new TaskCompletionSource<bool>();
#else
                    taskCompletionSource = new TaskCompletionSource();
#endif
                    notifyTask = taskCompletionSource.Task;
                }
            }
            finally
            {
                await subscriber.UnsubscribeAsync(_key + "_Notify");
            }
        }

        private async Task KeepAlive()
        {
            _keepAliveCTS = new CancellationTokenSource();
            while (_entered)
            {
                try
                {
                    await Task.Delay(30000, _keepAliveCTS.Token);
                }
                catch
                {
                    return;
                }
                if (_keepAliveCTS.Token.IsCancellationRequested)
                    return;
                try
                {
                    await _database.KeyExpireAsync(_key, TimeSpan.FromSeconds(10));
                }
                catch
                {

                }
            }
        }

        public async Task ExitAsync()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(RedisSemaphore));
            if (!_entered)
                throw new InvalidOperationException("Not entered.");
            _entered = false;
            _keepAliveCTS.Cancel();
            _keepAliveCTS = null;
            await _database.LockReleaseAsync(_key, _value);
            var subscriber = _connection.GetSubscriber();
            await subscriber.PublishAsync(_key + "_Notify", "release");
        }

        public async Task<bool> TryEnterAsync()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(RedisSemaphore));
            if (_entered)
                throw new InvalidOperationException("Already entered.");
            if (await _database.LockTakeAsync(_key, _value, TimeSpan.FromSeconds(30)))
            {
                _entered = true;
                _keepAliveTask = Task.Run(KeepAlive);
                return true;
            }
            return false;
        }

        private bool _disposed;
        public async ValueTask DisposeAsync()
        {
            if (_entered)
            {
                await ExitAsync();
            }
            _disposed = true;
        }
    }
}
