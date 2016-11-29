using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Redis
{
    public class RedisSemaphore : ISemaphore
    {
        private IDatabase _Database;
        private string _Key;

        public RedisSemaphore(IDatabase database, string key)
        {
            if (database == null)
                throw new ArgumentNullException(nameof(database));
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            _Database = database;
            _Key = key;
        }

        public void Dispose()
        {

        }

        public Task EnterAsync()
        {
            throw new NotImplementedException();
        }

        public Task<bool> EnterAsync(int timeout)
        {
            throw new NotImplementedException();
        }

        public Task ExitAsync()
        {
            throw new NotImplementedException();
        }

        public Task<bool> TryEnterAsync()
        {
            throw new NotImplementedException();
        }

        public Task<bool> TryEnterAsync(int timeout)
        {
            throw new NotImplementedException();
        }
    }
}
