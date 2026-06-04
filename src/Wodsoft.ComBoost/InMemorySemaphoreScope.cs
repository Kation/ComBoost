using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    public class InMemorySemaphoreScope : IDisposable
#if !NETSTANDARD2_0
        , IAsyncDisposable
#endif
    {
        private readonly SemaphoreSlim _semaphore;
        private int _disposed;

        public InMemorySemaphoreScope(SemaphoreSlim semaphore)
        {
            _semaphore = semaphore;
        }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref _disposed, 1) == 1)
                return;
            _semaphore.Release();
        }

#if !NETSTANDARD2_0
        public ValueTask DisposeAsync()
        {
            Dispose();
            return default;
        }
#endif
    }
}
