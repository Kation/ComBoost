using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Mock
{
    public class MockCacheProvider : ICacheProvider
    {
        private MockCache _Cache;
        public ICache GetCache()
        {
            if (_Cache == null)
                _Cache = new Mock.MockCache();
            return _Cache;
        }

        public ICache GetCache(string name)
        {
            throw new NotSupportedException();
        }
    }
}
