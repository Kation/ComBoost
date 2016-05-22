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

        Task ExecuteAsync(IDomainContext context, MethodInfo method);
        Task ExecuteAsync(IDomainContext context, Func<Task> method);
        Task ExecuteAsync<T1>(IDomainContext context, Func<T1, Task> method);
        Task ExecuteAsync<T1, T2>(IDomainContext context, Func<T1, T2, Task> method);
        Task ExecuteAsync<T1, T2, T3>(IDomainContext context, Func<T1, T2, T3, Task> method);
        Task ExecuteAsync<T1, T2, T3, T4>(IDomainContext context, Func<T1, T2, T3, T4, Task> method);
        Task ExecuteAsync<T1, T2, T3, T4, T5>(IDomainContext context, Func<T1, T2, T3, T4, T5, Task> method);
        Task ExecuteAsync<T1, T2, T3, T4, T5, T6>(IDomainContext context, Func<T1, T2, T3, T4, T5, T6, Task> method);
        Task ExecuteAsync<T1, T2, T3, T4, T5, T6, T7>(IDomainContext context, Func<T1, T2, T3, T4, T5, T6, T7, Task> method);
        Task ExecuteAsync<T1, T2, T3, T4, T5, T6, T7, T8>(IDomainContext context, Func<T1, T2, T3, T4, T5, T6, T7, T8, Task> method);
        Task ExecuteAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(IDomainContext context, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, Task> method);
        Task ExecuteAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(IDomainContext context, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, Task> method);
        Task ExecuteAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(IDomainContext context, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, Task> method);
        Task ExecuteAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(IDomainContext context, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, Task> method);
        Task ExecuteAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(IDomainContext context, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, Task> method);
        Task ExecuteAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(IDomainContext context, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, Task> method);
        Task ExecuteAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(IDomainContext context, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, Task> method);
        Task ExecuteAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(IDomainContext context, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, Task> method);

        event DomainExecuteEvent Executing;
        event DomainExecuteEvent Executed;
    }

    public delegate Task DomainExecuteEvent(IDomainExecutionContext context);
}
