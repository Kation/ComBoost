using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Distributed.RabbitMQ.Test.Services
{
    public interface IEventTestService : IDomainTemplate
    {
        Task Test(string text);
    }
}
