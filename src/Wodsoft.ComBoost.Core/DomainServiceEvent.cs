using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    public delegate void DomainServiceEvent(IDomainExecutionContext context);

    public delegate void DomainServiceEvent<T>(IDomainExecutionContext context, T e) where T : EventArgs;
}
