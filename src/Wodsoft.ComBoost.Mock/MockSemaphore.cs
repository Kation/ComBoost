using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Mock
{
    public class MockSemaphore : ISemaphore
    {
        private SemaphoreSlim _Semaphore;

        public MockSemaphore(SemaphoreSlim semaphore)
        {
            if (semaphore == null)
                throw new ArgumentNullException(nameof(semaphore));
            _Semaphore = semaphore;
        }

        public Task EnterAsync(CancellationToken cancellationToken = default)
        {
            return _Semaphore.WaitAsync(cancellationToken);
        }

        public Task<bool> EnterAsync(int timeout)
        {
            return _Semaphore.WaitAsync(timeout);
        }

        public Task ExitAsync()
        {
            _Semaphore.Release();
            return Task.CompletedTask;
        }

        public Task<bool> TryEnterAsync()
        {
            return _Semaphore.WaitAsync(0);
        }
    }
}
