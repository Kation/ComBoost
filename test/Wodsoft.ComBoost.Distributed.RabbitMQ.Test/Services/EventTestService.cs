using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Distributed.RabbitMQ.Test.Services
{
    public class EventTestService : DomainService
    {
        public Task FireHandleMore([FromValue] string text)
        {
            return RaiseEvent(new HandleMoreEventArgs { Text = text });
        }

        public Task FireHandleMoreDelay([FromValue] string text)
        {
            return RaiseEvent(new HandleMoreDelayEventArgs { Text = text, Delay = 2000 });
        }

        public Task FireHandleOnce([FromValue] string text)
        {
            return RaiseEvent(new HandleOnceEventArgs { Text = text });
        }

        public Task FireHandleOnceDelay([FromValue] string text)
        {
            return RaiseEvent(new HandleOnceDelayEventArgs { Text = text, Delay = 2000 });
        }

        public Task FireHandleGroup([FromValue] string text)
        {
            return RaiseEvent(new HandleGroupEventArgs { Text = text });
        }

        public Task FireHandleGroupDelay([FromValue] string text)
        {
            return RaiseEvent(new HandleGroupDelayEventArgs { Text = text, Delay = 2000 });
        }
    }
}
