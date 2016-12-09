using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Redis
{
    public class RedisSemaphore : ISemaphore
    {
        private IDatabase _Database;
        private string _Key;
        private TimeSpan _ExpireTime;

        public RedisSemaphore(IDatabase database, string key, TimeSpan expireTime)
        {
            if (database == null)
                throw new ArgumentNullException(nameof(database));
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            if (expireTime.TotalMilliseconds <= 0)
                throw new ArgumentOutOfRangeException(nameof(expireTime), "超时时间不能小于零。");
            _Database = database;
            _Key = key;
            _ExpireTime = expireTime;
        }

        public async Task EnterAsync()
        {
            while (true)
            {
                if (await TryEnterAsync())
                    return;
                await Task.Delay(10);
            }
        }

        public async Task<bool> EnterAsync(int timeout)
        {
            Stopwatch watch = new Stopwatch();
            while (watch.ElapsedMilliseconds < timeout)
            {
                if (await TryEnterAsync())
                    return true;
                await Task.Delay(10);
            }
            return false;
        }

        public Task ExitAsync()
        {
            return _Database.LockReleaseAsync(_Key, true);
        }

        public Task<bool> TryEnterAsync()
        {
            return _Database.LockTakeAsync(_Key, true, _ExpireTime);
        }
    }
}
