using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Distributed.CAP
{
    public class DomainCAPEventHandlerProvider
    {
        public Dictionary<Delegate, (string, string?)> Handlers = new Dictionary<Delegate, (string, string?)>();
    }
}
