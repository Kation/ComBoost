using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Distributed.CAP.Test.Services
{
    public interface IEventTestService : IDomainTemplate
    {
        Task FireHandleOnce(string text);

        Task FireHandleGroup(string text);
    }
}
