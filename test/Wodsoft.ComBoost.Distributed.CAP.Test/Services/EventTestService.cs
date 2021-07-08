using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Distributed.CAP.Test.Services
{
    public class EventTestService : DomainService
    {
        public Task Test([FromValue] string text)
        {
            return RaiseEvent(new TestEventArgs { Text = text });
        }
    }
}
