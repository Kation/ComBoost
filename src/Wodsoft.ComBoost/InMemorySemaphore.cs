using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    public class InMemorySemaphore : ISemaphore, IDisposable
    {
        private SemaphoreSlim _semaphore;
        private bool _entered, _disposed;

        public InMemorySemaphore(SemaphoreSlim semaphore)
        {
            if (semaphore == null)
                throw new ArgumentNullException(nameof(semaphore));
            _semaphore = semaphore;
        }

        public async Task EnterAsync(CancellationToken cancellationToken = default)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(InMemorySemaphore));
            if (_entered)
                throw new InvalidOperationException("Already entered.");
            await _semaphore.WaitAsync(cancellationToken);
            _entered = true;
        }

        public async Task<bool> EnterAsync(int timeout)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(InMemorySemaphore));
            if (_entered)
                throw new InvalidOperationException("Already entered.");
            if (await _semaphore.WaitAsync(timeout))
            {
                _entered = true;
                return true;
            }
            return false;
        }

        public Task ExitAsync()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(InMemorySemaphore));
            if (!_entered)
                throw new InvalidOperationException("Not entered.");
            _semaphore.Release();
            _entered = false;
            return Task.CompletedTask;
        }

        public async Task<bool> TryEnterAsync()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(InMemorySemaphore));
            if (_entered)
                throw new InvalidOperationException("Already entered.");
            if (await _semaphore.WaitAsync(0))
            {
                _entered = true;
                return true;
            }
            return false;
        }

        public void Dispose()
        {
            if (_disposed)
                return;
            _disposed = true;
            if (_entered)
            {
                _semaphore.Release();
                _entered = false;
            }
        }
    }
}
