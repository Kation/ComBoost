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
            return RaiseEvent(new HandleMoreDelayEventArgs { Text = text, Delay = 5000 });
        }

        public Task FireHandleMoreDelayPlugin([FromValue] string text, [FromValue] int delay)
        {
            return RaiseEvent(new HandleMoreDelayPluginEventArgs { Text = text, Delay = delay });
        }

        public Task FireHandleOnce([FromValue] string text)
        {
            return RaiseEvent(new HandleOnceEventArgs { Text = text });
        }

        public Task FireHandleOnceDelay([FromValue] string text)
        {
            return RaiseEvent(new HandleOnceDelayEventArgs { Text = text, Delay = 5000 });
        }

        public Task FireHandleGroup([FromValue] string text)
        {
            return RaiseEvent(new HandleGroupEventArgs { Text = text });
        }

        public Task FireHandleGroupDelay([FromValue] string text)
        {
            return RaiseEvent(new HandleGroupDelayEventArgs { Text = text, Delay = 5000 });
        }

        public Task FireHandleRetry([FromValue] string text)
        {
            return RaiseEvent(new HandleRetryEventArgs { Text = text });
        }

        public Task FireHandleSingle([FromValue] string text)
        {
            return RaiseEvent(new HandleSingleEventArgs { Text = text });
        }

        public Task FireHandleGroupSingle([FromValue] string text)
        {
            return RaiseEvent(new HandleGroupSingleEventArgs { Text = text });
        }

        public Task FireHandleRetryPlugin([FromValue] string text)
        {
            return RaiseEvent(new HandleRetryPluginEventArgs { Text = text });
        }
    }
}
