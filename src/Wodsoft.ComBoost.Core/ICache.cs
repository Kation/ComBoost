using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    public interface ICache
    {
        Task<object> GetAsync(string name, Type valueType);
        Task<bool> DeleteAsync(string name);
        Task SetAsync(string name, object value, TimeSpan? expireTime);
    }
}
