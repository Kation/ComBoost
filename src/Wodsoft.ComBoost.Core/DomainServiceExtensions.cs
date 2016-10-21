using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;

namespace Wodsoft.ComBoost
{
    public static class DomainServiceExtensions
    {
        public static Task ExecuteAsync(this IDomainService domainService, IDomainContext domainContext, Func<Task> method)
        {
            if (domainService == null)
                throw new ArgumentNullException(nameof(domainService));
            return domainService.ExecuteAsync(domainContext, method.GetMethodInfo());
        }
        public static Task ExecuteAsync<T1>(this IDomainService domainService, IDomainContext domainContext, Func<T1, Task> method)
        {
            if (domainService == null)
                throw new ArgumentNullException(nameof(domainService));
            return domainService.ExecuteAsync(domainContext, method.GetMethodInfo());
        }
        public static Task ExecuteAsync<T1, T2>(this IDomainService domainService, IDomainContext domainContext, Func<T1, T2, Task> method)
        {
            if (domainService == null)
                throw new ArgumentNullException(nameof(domainService));
            return domainService.ExecuteAsync(domainContext, method.GetMethodInfo());
        }
        public static Task ExecuteAsync<T1, T2, T3>(this IDomainService domainService, IDomainContext domainContext, Func<T1, T2, T3, Task> method)
        {
            if (domainService == null)
                throw new ArgumentNullException(nameof(domainService));
            return domainService.ExecuteAsync(domainContext, method.GetMethodInfo());
        }
        public static Task ExecuteAsync<T1, T2, T3, T4>(this IDomainService domainService, IDomainContext domainContext, Func<T1, T2, T3, T4, Task> method)
        {
            if (domainService == null)
                throw new ArgumentNullException(nameof(domainService));
            return domainService.ExecuteAsync(domainContext, method.GetMethodInfo());
        }
        public static Task ExecuteAsync<T1, T2, T3, T4, T5>(this IDomainService domainService, IDomainContext domainContext, Func<T1, T2, T3, T4, T5, Task> method)
        {
            if (domainService == null)
                throw new ArgumentNullException(nameof(domainService));
            return domainService.ExecuteAsync(domainContext, method.GetMethodInfo());
        }
        public static Task ExecuteAsync<T1, T2, T3, T4, T5, T6>(this IDomainService domainService, IDomainContext domainContext, Func<T1, T2, T3, T4, T5, T6, Task> method)
        {
            if (domainService == null)
                throw new ArgumentNullException(nameof(domainService));
            return domainService.ExecuteAsync(domainContext, method.GetMethodInfo());
        }
        public static Task ExecuteAsync<T1, T2, T3, T4, T5, T6, T7>(this IDomainService domainService, IDomainContext domainContext, Func<T1, T2, T3, T4, T5, T6, T7, Task> method)
        {
            if (domainService == null)
                throw new ArgumentNullException(nameof(domainService));
            return domainService.ExecuteAsync(domainContext, method.GetMethodInfo());
        }
        public static Task ExecuteAsync<T1, T2, T3, T4, T5, T6, T7, T8>(this IDomainService domainService, IDomainContext domainContext, Func<T1, T2, T3, T4, T5, T6, T7, T8, Task> method)
        {
            if (domainService == null)
                throw new ArgumentNullException(nameof(domainService));
            return domainService.ExecuteAsync(domainContext, method.GetMethodInfo());
        }
        public static Task ExecuteAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this IDomainService domainService, IDomainContext domainContext, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, Task> method)
        {
            if (domainService == null)
                throw new ArgumentNullException(nameof(domainService));
            return domainService.ExecuteAsync(domainContext, method.GetMethodInfo());
        }
        public static Task ExecuteAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this IDomainService domainService, IDomainContext domainContext, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, Task> method)
        {
            if (domainService == null)
                throw new ArgumentNullException(nameof(domainService));
            return domainService.ExecuteAsync(domainContext, method.GetMethodInfo());
        }
        public static Task ExecuteAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this IDomainService domainService, IDomainContext domainContext, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, Task> method)
        {
            if (domainService == null)
                throw new ArgumentNullException(nameof(domainService));
            return domainService.ExecuteAsync(domainContext, method.GetMethodInfo());
        }
        public static Task ExecuteAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this IDomainService domainService, IDomainContext domainContext, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, Task> method)
        {
            if (domainService == null)
                throw new ArgumentNullException(nameof(domainService));
            return domainService.ExecuteAsync(domainContext, method.GetMethodInfo());
        }
        public static Task ExecuteAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this IDomainService domainService, IDomainContext domainContext, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, Task> method)
        {
            if (domainService == null)
                throw new ArgumentNullException(nameof(domainService));
            return domainService.ExecuteAsync(domainContext, method.GetMethodInfo());
        }
        public static Task ExecuteAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this IDomainService domainService, IDomainContext domainContext, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, Task> method)
        {
            if (domainService == null)
                throw new ArgumentNullException(nameof(domainService));
            return domainService.ExecuteAsync(domainContext, method.GetMethodInfo());
        }
        public static Task ExecuteAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this IDomainService domainService, IDomainContext domainContext, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, Task> method)
        {
            if (domainService == null)
                throw new ArgumentNullException(nameof(domainService));
            return domainService.ExecuteAsync(domainContext, method.GetMethodInfo());
        }
        public static Task ExecuteAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(this IDomainService domainService, IDomainContext domainContext, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, Task> method)
        {
            if (domainService == null)
                throw new ArgumentNullException(nameof(domainService));
            return domainService.ExecuteAsync(domainContext, method.GetMethodInfo());
        }

        public static Task<TResult> ExecuteAsync<TResult>(this IDomainService domainService, IDomainContext domainContext, Func<Task<TResult>> method)
        {
            if (domainService == null)
                throw new ArgumentNullException(nameof(domainService));
            return domainService.ExecuteAsync<TResult>(domainContext, method.GetMethodInfo());
        }
        public static Task<TResult> ExecuteAsync<T1, TResult>(this IDomainService domainService, IDomainContext domainContext, Func<T1, Task<TResult>> method)
        {
            if (domainService == null)
                throw new ArgumentNullException(nameof(domainService));
            return domainService.ExecuteAsync<TResult>(domainContext, method.GetMethodInfo());
        }
        public static Task<TResult> ExecuteAsync<T1, T2, TResult>(this IDomainService domainService, IDomainContext domainContext, Func<T1, T2, Task<TResult>> method)
        {
            if (domainService == null)
                throw new ArgumentNullException(nameof(domainService));
            return domainService.ExecuteAsync<TResult>(domainContext, method.GetMethodInfo());
        }
        public static Task<TResult> ExecuteAsync<T1, T2, T3, TResult>(this IDomainService domainService, IDomainContext domainContext, Func<T1, T2, T3, Task<TResult>> method)
        {
            if (domainService == null)
                throw new ArgumentNullException(nameof(domainService));
            return domainService.ExecuteAsync<TResult>(domainContext, method.GetMethodInfo());
        }
        public static Task<TResult> ExecuteAsync<T1, T2, T3, T4, TResult>(this IDomainService domainService, IDomainContext domainContext, Func<T1, T2, T3, T4, Task<TResult>> method)
        {
            if (domainService == null)
                throw new ArgumentNullException(nameof(domainService));
            return domainService.ExecuteAsync<TResult>(domainContext, method.GetMethodInfo());
        }
        public static Task<TResult> ExecuteAsync<T1, T2, T3, T4, T5, TResult>(this IDomainService domainService, IDomainContext domainContext, Func<T1, T2, T3, T4, T5, Task<TResult>> method)
        {
            if (domainService == null)
                throw new ArgumentNullException(nameof(domainService));
            return domainService.ExecuteAsync<TResult>(domainContext, method.GetMethodInfo());
        }
        public static Task<TResult> ExecuteAsync<T1, T2, T3, T4, T5, T6, TResult>(this IDomainService domainService, IDomainContext domainContext, Func<T1, T2, T3, T4, T5, T6, Task<TResult>> method)
        {
            if (domainService == null)
                throw new ArgumentNullException(nameof(domainService));
            return domainService.ExecuteAsync<TResult>(domainContext, method.GetMethodInfo());
        }
        public static Task<TResult> ExecuteAsync<T1, T2, T3, T4, T5, T6, T7, TResult>(this IDomainService domainService, IDomainContext domainContext, Func<T1, T2, T3, T4, T5, T6, T7, Task<TResult>> method)
        {
            if (domainService == null)
                throw new ArgumentNullException(nameof(domainService));
            return domainService.ExecuteAsync<TResult>(domainContext, method.GetMethodInfo());
        }
        public static Task<TResult> ExecuteAsync<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this IDomainService domainService, IDomainContext domainContext, Func<T1, T2, T3, T4, T5, T6, T7, T8, Task<TResult>> method)
        {
            if (domainService == null)
                throw new ArgumentNullException(nameof(domainService));
            return domainService.ExecuteAsync<TResult>(domainContext, method.GetMethodInfo());
        }
        public static Task<TResult> ExecuteAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(this IDomainService domainService, IDomainContext domainContext, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, Task<TResult>> method)
        {
            if (domainService == null)
                throw new ArgumentNullException(nameof(domainService));
            return domainService.ExecuteAsync<TResult>(domainContext, method.GetMethodInfo());
        }
        public static Task<TResult> ExecuteAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(this IDomainService domainService, IDomainContext domainContext, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, Task<TResult>> method)
        {
            if (domainService == null)
                throw new ArgumentNullException(nameof(domainService));
            return domainService.ExecuteAsync<TResult>(domainContext, method.GetMethodInfo());
        }
        public static Task<TResult> ExecuteAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(this IDomainService domainService, IDomainContext domainContext, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, Task<TResult>> method)
        {
            if (domainService == null)
                throw new ArgumentNullException(nameof(domainService));
            return domainService.ExecuteAsync<TResult>(domainContext, method.GetMethodInfo());
        }
        public static Task<TResult> ExecuteAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(this IDomainService domainService, IDomainContext domainContext, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, Task<TResult>> method)
        {
            if (domainService == null)
                throw new ArgumentNullException(nameof(domainService));
            return domainService.ExecuteAsync<TResult>(domainContext, method.GetMethodInfo());
        }
        public static Task<TResult> ExecuteAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(this IDomainService domainService, IDomainContext domainContext, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, Task<TResult>> method)
        {
            if (domainService == null)
                throw new ArgumentNullException(nameof(domainService));
            return domainService.ExecuteAsync<TResult>(domainContext, method.GetMethodInfo());
        }
        public static Task<TResult> ExecuteAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(this IDomainService domainService, IDomainContext domainContext, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, Task<TResult>> method)
        {
            if (domainService == null)
                throw new ArgumentNullException(nameof(domainService));
            return domainService.ExecuteAsync<TResult>(domainContext, method.GetMethodInfo());
        }
        public static Task<TResult> ExecuteAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(this IDomainService domainService, IDomainContext domainContext, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, Task<TResult>> method)
        {
            if (domainService == null)
                throw new ArgumentNullException(nameof(domainService));
            return domainService.ExecuteAsync<TResult>(domainContext, method.GetMethodInfo());
        }
        public static Task<TResult> ExecuteAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(this IDomainService domainService, IDomainContext domainContext, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, Task<TResult>> method)
        {
            if (domainService == null)
                throw new ArgumentNullException(nameof(domainService));
            return domainService.ExecuteAsync<TResult>(domainContext, method.GetMethodInfo());
        }

        public static Task ExecuteAsync(this IDomainService domainService, IDomainContext domainContext, string method)
        {
            if (domainService == null)
                throw new ArgumentNullException(nameof(domainService));
            return domainService.ExecuteAsync(domainContext, domainService.GetType().GetMethod(method));
        }

        public static Task<TResult> ExecuteAsync<TResult>(this IDomainService domainService, IDomainContext domainContext, string method)
        {
            if (domainService == null)
                throw new ArgumentNullException(nameof(domainService));
            return domainService.ExecuteAsync<TResult>(domainContext, domainService.GetType().GetMethod(method));
        }
    }
}
