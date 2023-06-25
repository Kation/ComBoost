using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Distributed.RabbitMQ.Test.Services
{
    public interface IEventTestService : IDomainTemplate
    {
        Task FireHandleOnce(string text);

        Task FireHandleMore(string text);

        Task FireHandleGroup(string text);

        Task FireHandleOnceDelay(string text);

        Task FireHandleMoreDelay(string text);

        Task FireHandleMoreDelayPlugin(string text, int delay);

        Task FireHandleGroupDelay(string text);

        Task FireHandleRetry(string text);

        Task FireHandleRetryPlugin(string text);

        Task FireHandleSingle(string text);

        Task FireHandleGroupSingle(string text);
    }
}
