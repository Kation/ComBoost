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

        Task FireHandleGroupDelay(string text);

        Task FireHandleOnceRetry(string text);
    }
}
