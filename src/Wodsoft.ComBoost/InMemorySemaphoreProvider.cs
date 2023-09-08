using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;

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
    }
}
