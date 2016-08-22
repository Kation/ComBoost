using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    public interface IDomainService
    {
        IDomainExecutionContext Context { get; }

        Task ExecuteAsync(IDomainContext domainContext, MethodInfo method);
        Task<T> ExecuteAsync<T>(IDomainContext domainContext, MethodInfo method);

        event DomainExecuteEvent Executing;
        event DomainExecuteEvent Executed;
    }

    public delegate Task DomainExecuteEvent(IDomainExecutionContext context);
}
