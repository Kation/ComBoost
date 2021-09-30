using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Distributed.CAP.Test.Services
{
    public class EventTestService : DomainService
    {
        public Task FireHandleOnce([FromValue] string text)
        {
            return RaiseEvent(new HandleOnceEventArgs { Text = text });
        }

        public Task FireHandleGroup([FromValue] string text)
        {
            return RaiseEvent(new HandleGroupEventArgs { Text = text });
        }
    }
}
