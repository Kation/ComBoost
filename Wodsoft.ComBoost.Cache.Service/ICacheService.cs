using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Cache.Service
{
    [ServiceContract]
    public interface ICacheService
    {
        bool Login(string username, string password);

        void Logout();
    }
}
