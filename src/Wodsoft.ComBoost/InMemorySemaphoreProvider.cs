using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    public class InMemorySemaphoreProvider : ISemaphoreProvider
    {
        private ConcurrentDictionary<string, SemaphoreSlim> _cache;

        public InMemorySemaphoreProvider()
        {
            _cache = new ConcurrentDictionary<string, SemaphoreSlim>();
        }

        public ISemaphore GetSemaphore(string name)
        {
            var semaphore = _cache.GetOrAdd(name, key => new SemaphoreSlim(1));
            return new InMemorySemaphore(semaphore);
        }

#if NETSTANDARD2_0
        public async Task<IDisposable> EnterScopeAsync(string name, CancellationToken cancellationToken = default)
#else
        public async Task<IAsyncDisposable> EnterScopeAsync(string name, CancellationToken cancellationToken = default)
#endif
        {
            var semaphore = _cache.GetOrAdd(name, key => new SemaphoreSlim(1));
            await semaphore.WaitAsync(cancellationToken);
            return new InMemorySemaphoreScope(semaphore);
        }
    }
}
