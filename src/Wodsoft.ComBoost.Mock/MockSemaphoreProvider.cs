using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Mock
{
    public class MockSemaphoreProvider : ISemaphoreProvider
    {
        private ConcurrentDictionary<string, ISemaphore> _Cache;

        public MockSemaphoreProvider()
        {
            _Cache = new ConcurrentDictionary<string, ISemaphore>();
        }

        public ISemaphore GetSemaphore(string name)
        {
            return _Cache.GetOrAdd(name, key => new MockSemaphore(new SemaphoreSlim(1)));
        }
    }
}
