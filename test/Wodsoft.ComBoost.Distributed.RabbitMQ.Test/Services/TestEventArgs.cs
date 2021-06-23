using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Distributed.RabbitMQ.Test.Services
{
    public class TestEventArgs : DomainServiceEventArgs
    {
        public string Text { get; set; }
    }
}
