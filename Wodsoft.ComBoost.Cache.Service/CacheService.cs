using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Cache.Service
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public abstract class CacheService : ICacheService, IDisposable
    {
        public void Dispose()
        {

        }

        public abstract bool Login(string username, string password);

        public abstract void Logout();
    }
}
