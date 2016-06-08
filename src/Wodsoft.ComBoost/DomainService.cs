using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    public class DomainService : IDomainService
    {
        private DomainExecutionContext _Context;
        public IDomainExecutionContext ExecutionContext { get { return _Context; } }

        public event DomainExecuteEvent Executed;
        public event DomainExecuteEvent Executing;

        public async Task ExecuteAsync(IDomainContext domainContext, MethodInfo method)
        {
            if (domainContext == null)
                throw new ArgumentNullException(nameof(domainContext));
            await OnExecuting(domainContext, method);
            await (Task)method.Invoke(this, _Context.ParameterValues);
            await OnExecuted();
        }
        public async Task ExecuteAsync(IDomainContext domainContext, Func<Task> method)
        {
            if (domainContext == null)
                throw new ArgumentNullException(nameof(domainContext));
            await OnExecuting(domainContext, method.GetMethodInfo());
            await method();
            await OnExecuted();
        }
        public async Task ExecuteAsync<T1>(IDomainContext domainContext, Func<T1, Task> method)
        {
            if (domainContext == null)
                throw new ArgumentNullException(nameof(domainContext));
            await OnExecuting(domainContext, method.GetMethodInfo());
            await (Task)method.DynamicInvoke(_Context.ParameterValues);
            await OnExecuted();
        }
        public async Task ExecuteAsync<T1, T2>(IDomainContext domainContext, Func<T1, T2, Task> method)
        {
            if (domainContext == null)
                throw new ArgumentNullException(nameof(domainContext));
            await OnExecuting(domainContext, method.GetMethodInfo());
            await (Task)method.DynamicInvoke(_Context.ParameterValues);
            await OnExecuted();
        }
        public async Task ExecuteAsync<T1, T2, T3>(IDomainContext domainContext, Func<T1, T2, T3, Task> method)
        {
            if (domainContext == null)
                throw new ArgumentNullException(nameof(domainContext));
            await OnExecuting(domainContext, method.GetMethodInfo());
            await (Task)method.DynamicInvoke(_Context.ParameterValues);
            await OnExecuted();
        }
        public async Task ExecuteAsync<T1, T2, T3, T4>(IDomainContext domainContext, Func<T1, T2, T3, T4, Task> method)
        {
            if (domainContext == null)
                throw new ArgumentNullException(nameof(domainContext));
            await OnExecuting(domainContext, method.GetMethodInfo());
            await (Task)method.DynamicInvoke(_Context.ParameterValues);
            await OnExecuted();
        }
        public async Task ExecuteAsync<T1, T2, T3, T4, T5>(IDomainContext domainContext, Func<T1, T2, T3, T4, T5, Task> method)
        {
            if (domainContext == null)
                throw new ArgumentNullException(nameof(domainContext));
            await OnExecuting(domainContext, method.GetMethodInfo());
            await (Task)method.DynamicInvoke(_Context.ParameterValues);
            await OnExecuted();
        }
        public async Task ExecuteAsync<T1, T2, T3, T4, T5, T6>(IDomainContext domainContext, Func<T1, T2, T3, T4, T5, T6, Task> method)
        {
            if (domainContext == null)
                throw new ArgumentNullException(nameof(domainContext));
            await OnExecuting(domainContext, method.GetMethodInfo());
            await (Task)method.DynamicInvoke(_Context.ParameterValues);
            await OnExecuted();
        }
        public async Task ExecuteAsync<T1, T2, T3, T4, T5, T6, T7>(IDomainContext domainContext, Func<T1, T2, T3, T4, T5, T6, T7, Task> method)
        {
            if (domainContext == null)
                throw new ArgumentNullException(nameof(domainContext));
            await OnExecuting(domainContext, method.GetMethodInfo());
            await (Task)method.DynamicInvoke(_Context.ParameterValues);
            await OnExecuted();
        }
        public async Task ExecuteAsync<T1, T2, T3, T4, T5, T6, T7, T8>(IDomainContext domainContext, Func<T1, T2, T3, T4, T5, T6, T7, T8, Task> method)
        {
            if (domainContext == null)
                throw new ArgumentNullException(nameof(domainContext));
            await OnExecuting(domainContext, method.GetMethodInfo());
            await (Task)method.DynamicInvoke(_Context.ParameterValues);
            await OnExecuted();
        }
        public async Task ExecuteAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(IDomainContext domainContext, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, Task> method)
        {
            if (domainContext == null)
                throw new ArgumentNullException(nameof(domainContext));
            await OnExecuting(domainContext, method.GetMethodInfo());
            await (Task)method.DynamicInvoke(_Context.ParameterValues);
            await OnExecuted();
        }
        public async Task ExecuteAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(IDomainContext domainContext, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, Task> method)
        {
            if (domainContext == null)
                throw new ArgumentNullException(nameof(domainContext));
            await OnExecuting(domainContext, method.GetMethodInfo());
            await (Task)method.DynamicInvoke(_Context.ParameterValues);
            await OnExecuted();
        }
        public async Task ExecuteAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(IDomainContext domainContext, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, Task> method)
        {
            if (domainContext == null)
                throw new ArgumentNullException(nameof(domainContext));
            await OnExecuting(domainContext, method.GetMethodInfo());
            await (Task)method.DynamicInvoke(_Context.ParameterValues);
            await OnExecuted();
        }
        public async Task ExecuteAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(IDomainContext domainContext, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, Task> method)
        {
            if (domainContext == null)
                throw new ArgumentNullException(nameof(domainContext));
            await OnExecuting(domainContext, method.GetMethodInfo());
            await (Task)method.DynamicInvoke(_Context.ParameterValues);
            await OnExecuted();
        }
        public async Task ExecuteAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(IDomainContext domainContext, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, Task> method)
        {
            if (domainContext == null)
                throw new ArgumentNullException(nameof(domainContext));
            await OnExecuting(domainContext, method.GetMethodInfo());
            await (Task)method.DynamicInvoke(_Context.ParameterValues);
            await OnExecuted();
        }
        public async Task ExecuteAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(IDomainContext domainContext, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, Task> method)
        {
            if (domainContext == null)
                throw new ArgumentNullException(nameof(domainContext));
            await OnExecuting(domainContext, method.GetMethodInfo());
            await (Task)method.DynamicInvoke(_Context.ParameterValues);
            await OnExecuted();
        }
        public async Task ExecuteAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(IDomainContext domainContext, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, Task> method)
        {
            if (domainContext == null)
                throw new ArgumentNullException(nameof(domainContext));
            await OnExecuting(domainContext, method.GetMethodInfo());
            await (Task)method.DynamicInvoke(_Context.ParameterValues);
            await OnExecuted();
        }
        public async Task ExecuteAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(IDomainContext domainContext, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, Task> method)
        {
            if (domainContext == null)
                throw new ArgumentNullException(nameof(domainContext));
            await OnExecuting(domainContext, method.GetMethodInfo());
            await (Task)method.DynamicInvoke(_Context.ParameterValues);
            await OnExecuted();
        }
        private async Task OnExecuting(IDomainContext domainContext, MethodInfo method)
        {
            _Context = new DomainExecutionContext(this, domainContext, method);
            if (Executing != null)
                await Executing(_Context);
        }
        private async Task OnExecuted()
        {
            if (Executed != null)
                await Executed(_Context);
            _Context = null;
        }
    }
}
