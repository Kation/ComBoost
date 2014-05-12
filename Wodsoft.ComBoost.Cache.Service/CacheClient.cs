using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Cache.Service
{
    public class CacheClient<TService> : ClientBase<TService>, ICacheService where TService : class, ICacheService
    {

        public bool Login(string username, string password)
        {
            throw new NotImplementedException();
        }

        public void Logout()
        {
            throw new NotImplementedException();
        }
    }
}
