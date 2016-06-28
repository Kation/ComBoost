using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    public interface IDomainService
    {
        IDomainExecutionContext ExecutionContext { get; }

        Task ExecuteAsync(IDomainContext context, Delegate method);
        
        event DomainExecuteEvent Executing;
        event DomainExecuteEvent Executed;
    }

    public delegate Task DomainExecuteEvent(IDomainExecutionContext context);
}
