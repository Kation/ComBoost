using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using System.Collections.Concurrent;

namespace Wodsoft.ComBoost
{
    /// <summary>
    /// 领域服务扩展。
    /// </summary>
    public static class DomainServiceExtensions
    {
        /// <summary>
        /// 异步执行领域方法。
        /// </summary>
        /// <param name="domainService">领域服务。</param>
        /// <param name="domainContext">领域上下文。</param>
        /// <param name="method">领域方法。</param>
        /// <returns></returns>
        public static Task ExecuteAsync(this IDomainService domainService, IDomainContext domainContext, Func<Task> method)
        {
            if (domainService == null)
                throw new ArgumentNullException(nameof(domainService));
            return domainService.ExecuteAsync(domainContext, method.GetMethodInfo());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="domainService">领域服务。</param>
        /// <param name="domainContext">领域上下文。</param>
        /// <param name="method">领域方法。</param>
        /// <returns></returns>
        public static Task ExecuteAsync<T1>(this IDomainService domainService, IDomainContext domainContext, Func<T1, Task> method)
        {
            if (domainService == null)
                throw new ArgumentNullException(nameof(domainService));
            return domainService.ExecuteAsync(domainContext, method.GetMethodInfo());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="domainService"></param>
        /// <param name="domainContext"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public static Task ExecuteAsync<T1, T2>(this IDomainService domainService, IDomainContext domainContext, Func<T1, T2, Task> method)
        {
            if (domainService == null)
                throw new ArgumentNullException(nameof(domainService));
            return domainService.ExecuteAsync(domainContext, method.GetMethodInfo());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <param name="domainService">领域服务。</param>
        /// <param name="domainContext">领域上下文。</param>
        /// <param name="method">领域方法。</param>
        /// <returns></returns>
        public static Task ExecuteAsync<T1, T2, T3>(this IDomainService domainService, IDomainContext domainContext, Func<T1, T2, T3, Task> method)
        {
            if (domainService == null)
                throw new ArgumentNullException(nameof(domainService));
            return domainService.ExecuteAsync(domainContext, method.GetMethodInfo());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <param name="domainService">领域服务。</param>
        /// <param name="domainContext">领域上下文。</param>
        /// <param name="method">领域方法。</param>
        /// <returns></returns>
        public static Task ExecuteAsync<T1, T2, T3, T4>(this IDomainService domainService, IDomainContext domainContext, Func<T1, T2, T3, T4, Task> method)
        {
            if (domainService == null)
                throw new ArgumentNullException(nameof(domainService));
            return domainService.ExecuteAsync(domainContext, method.GetMethodInfo());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <param name="domainService">领域服务。</param>
        /// <param name="domainContext">领域上下文。</param>
        /// <param name="method">领域方法。</param>
        /// <returns></returns>
        public static Task ExecuteAsync<T1, T2, T3, T4, T5>(this IDomainService domainService, IDomainContext domainContext, Func<T1, T2, T3, T4, T5, Task> method)
        {
            if (domainService == null)
                throw new ArgumentNullException(nameof(domainService));
            return domainService.ExecuteAsync(domainContext, method.GetMethodInfo());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <param name="domainService">领域服务。</param>
        /// <param name="domainContext">领域上下文。</param>
        /// <param name="method">领域方法。</param>
        /// <returns></returns>
        public static Task ExecuteAsync<T1, T2, T3, T4, T5, T6>(this IDomainService domainService, IDomainContext domainContext, Func<T1, T2, T3, T4, T5, T6, Task> method)
        {
            if (domainService == null)
                throw new ArgumentNullException(nameof(domainService));
            return domainService.ExecuteAsync(domainContext, method.GetMethodInfo());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <param name="domainService">领域服务。</param>
        /// <param name="domainContext">领域上下文。</param>
        /// <param name="method">领域方法。</param>
        /// <returns></returns>
        public static Task ExecuteAsync<T1, T2, T3, T4, T5, T6, T7>(this IDomainService domainService, IDomainContext domainContext, Func<T1, T2, T3, T4, T5, T6, T7, Task> method)
        {
            if (domainService == null)
                throw new ArgumentNullException(nameof(domainService));
            return domainService.ExecuteAsync(domainContext, method.GetMethodInfo());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <typeparam name="T8"></typeparam>
        /// <param name="domainService">领域服务。</param>
        /// <param name="domainContext">领域上下文。</param>
        /// <param name="method">领域方法。</param>
        /// <returns></returns>
        public static Task ExecuteAsync<T1, T2, T3, T4, T5, T6, T7, T8>(this IDomainService domainService, IDomainContext domainContext, Func<T1, T2, T3, T4, T5, T6, T7, T8, Task> method)
        {
            if (domainService == null)
                throw new ArgumentNullException(nameof(domainService));
            return domainService.ExecuteAsync(domainContext, method.GetMethodInfo());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <typeparam name="T8"></typeparam>
        /// <typeparam name="T9"></typeparam>
        /// <param name="domainService">领域服务。</param>
        /// <param name="domainContext">领域上下文。</param>
        /// <param name="method">领域方法。</param>
        /// <returns></returns>
        public static Task ExecuteAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this IDomainService domainService, IDomainContext domainContext, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, Task> method)
        {
            if (domainService == null)
                throw new ArgumentNullException(nameof(domainService));
            return domainService.ExecuteAsync(domainContext, method.GetMethodInfo());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <typeparam name="T8"></typeparam>
        /// <typeparam name="T9"></typeparam>
        /// <typeparam name="T10"></typeparam>
        /// <param name="domainService">领域服务。</param>
        /// <param name="domainContext">领域上下文。</param>
        /// <param name="method">领域方法。</param>
        /// <returns></returns>
        public static Task ExecuteAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this IDomainService domainService, IDomainContext domainContext, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, Task> method)
        {
            if (domainService == null)
                throw new ArgumentNullException(nameof(domainService));
            return domainService.ExecuteAsync(domainContext, method.GetMethodInfo());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <typeparam name="T8"></typeparam>
        /// <typeparam name="T9"></typeparam>
        /// <typeparam name="T10"></typeparam>
        /// <typeparam name="T11"></typeparam>
        /// <param name="domainService">领域服务。</param>
        /// <param name="domainContext">领域上下文。</param>
        /// <param name="method">领域方法。</param>
        /// <returns></returns>
        public static Task ExecuteAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this IDomainService domainService, IDomainContext domainContext, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, Task> method)
        {
            if (domainService == null)
                throw new ArgumentNullException(nameof(domainService));
            return domainService.ExecuteAsync(domainContext, method.GetMethodInfo());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <typeparam name="T8"></typeparam>
        /// <typeparam name="T9"></typeparam>
        /// <typeparam name="T10"></typeparam>
        /// <typeparam name="T11"></typeparam>
        /// <typeparam name="T12"></typeparam>
        /// <param name="domainService">领域服务。</param>
        /// <param name="domainContext">领域上下文。</param>
        /// <param name="method">领域方法。</param>
        /// <returns></returns>
        public static Task ExecuteAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this IDomainService domainService, IDomainContext domainContext, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, Task> method)
        {
            if (domainService == null)
                throw new ArgumentNullException(nameof(domainService));
            return domainService.ExecuteAsync(domainContext, method.GetMethodInfo());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <typeparam name="T8"></typeparam>
        /// <typeparam name="T9"></typeparam>
        /// <typeparam name="T10"></typeparam>
        /// <typeparam name="T11"></typeparam>
        /// <typeparam name="T12"></typeparam>
        /// <typeparam name="T13"></typeparam>
        /// <param name="domainService">领域服务。</param>
        /// <param name="domainContext">领域上下文。</param>
        /// <param name="method">领域方法。</param>
        /// <returns></returns>
        public static Task ExecuteAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this IDomainService domainService, IDomainContext domainContext, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, Task> method)
        {
            if (domainService == null)
                throw new ArgumentNullException(nameof(domainService));
            return domainService.ExecuteAsync(domainContext, method.GetMethodInfo());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <typeparam name="T8"></typeparam>
        /// <typeparam name="T9"></typeparam>
        /// <typeparam name="T10"></typeparam>
        /// <typeparam name="T11"></typeparam>
        /// <typeparam name="T12"></typeparam>
        /// <typeparam name="T13"></typeparam>
        /// <typeparam name="T14"></typeparam>
        /// <param name="domainService">领域服务。</param>
        /// <param name="domainContext">领域上下文。</param>
        /// <param name="method">领域方法。</param>
        /// <returns></returns>
        public static Task ExecuteAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this IDomainService domainService, IDomainContext domainContext, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, Task> method)
        {
            if (domainService == null)
                throw new ArgumentNullException(nameof(domainService));
            return domainService.ExecuteAsync(domainContext, method.GetMethodInfo());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <typeparam name="T8"></typeparam>
        /// <typeparam name="T9"></typeparam>
        /// <typeparam name="T10"></typeparam>
        /// <typeparam name="T11"></typeparam>
        /// <typeparam name="T12"></typeparam>
        /// <typeparam name="T13"></typeparam>
        /// <typeparam name="T14"></typeparam>
        /// <typeparam name="T15"></typeparam>
        /// <param name="domainService">领域服务。</param>
        /// <param name="domainContext">领域上下文。</param>
        /// <param name="method">领域方法。</param>
        /// <returns></returns>
        public static Task ExecuteAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this IDomainService domainService, IDomainContext domainContext, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, Task> method)
        {
            if (domainService == null)
                throw new ArgumentNullException(nameof(domainService));
            return domainService.ExecuteAsync(domainContext, method.GetMethodInfo());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <typeparam name="T8"></typeparam>
        /// <typeparam name="T9"></typeparam>
        /// <typeparam name="T10"></typeparam>
        /// <typeparam name="T11"></typeparam>
        /// <typeparam name="T12"></typeparam>
        /// <typeparam name="T13"></typeparam>
        /// <typeparam name="T14"></typeparam>
        /// <typeparam name="T15"></typeparam>
        /// <typeparam name="T16"></typeparam>
        /// <param name="domainService">领域服务。</param>
        /// <param name="domainContext">领域上下文。</param>
        /// <param name="method">领域方法。</param>
        /// <returns></returns>
        public static Task ExecuteAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(this IDomainService domainService, IDomainContext domainContext, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, Task> method)
        {
            if (domainService == null)
                throw new ArgumentNullException(nameof(domainService));
            return domainService.ExecuteAsync(domainContext, method.GetMethodInfo());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResult">直接结果类型。</typeparam>
        /// <param name="domainService">领域服务。</param>
        /// <param name="domainContext">领域上下文。</param>
        /// <param name="method">领域方法。</param>
        /// <returns></returns>
        public static Task<TResult> ExecuteAsync<TResult>(this IDomainService domainService, IDomainContext domainContext, Func<Task<TResult>> method)
        {
            if (domainService == null)
                throw new ArgumentNullException(nameof(domainService));
            return domainService.ExecuteAsync<TResult>(domainContext, method.GetMethodInfo());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="TResult">直接结果类型。</typeparam>
        /// <param name="domainService">领域服务。</param>
        /// <param name="domainContext">领域上下文。</param>
        /// <param name="method">领域方法。</param>
        /// <returns></returns>
        public static Task<TResult> ExecuteAsync<T1, TResult>(this IDomainService domainService, IDomainContext domainContext, Func<T1, Task<TResult>> method)
        {
            if (domainService == null)
                throw new ArgumentNullException(nameof(domainService));
            return domainService.ExecuteAsync<TResult>(domainContext, method.GetMethodInfo());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="TResult">直接结果类型。</typeparam>
        /// <param name="domainService">领域服务。</param>
        /// <param name="domainContext">领域上下文。</param>
        /// <param name="method">领域方法。</param>
        /// <returns></returns>
        public static Task<TResult> ExecuteAsync<T1, T2, TResult>(this IDomainService domainService, IDomainContext domainContext, Func<T1, T2, Task<TResult>> method)
        {
            if (domainService == null)
                throw new ArgumentNullException(nameof(domainService));
            return domainService.ExecuteAsync<TResult>(domainContext, method.GetMethodInfo());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="TResult">直接结果类型。</typeparam>
        /// <param name="domainService">领域服务。</param>
        /// <param name="domainContext">领域上下文。</param>
        /// <param name="method">领域方法。</param>
        /// <returns></returns>
        public static Task<TResult> ExecuteAsync<T1, T2, T3, TResult>(this IDomainService domainService, IDomainContext domainContext, Func<T1, T2, T3, Task<TResult>> method)
        {
            if (domainService == null)
                throw new ArgumentNullException(nameof(domainService));
            return domainService.ExecuteAsync<TResult>(domainContext, method.GetMethodInfo());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="TResult">直接结果类型。</typeparam>
        /// <param name="domainService">领域服务。</param>
        /// <param name="domainContext">领域上下文。</param>
        /// <param name="method">领域方法。</param>
        /// <returns></returns>
        public static Task<TResult> ExecuteAsync<T1, T2, T3, T4, TResult>(this IDomainService domainService, IDomainContext domainContext, Func<T1, T2, T3, T4, Task<TResult>> method)
        {
            if (domainService == null)
                throw new ArgumentNullException(nameof(domainService));
            return domainService.ExecuteAsync<TResult>(domainContext, method.GetMethodInfo());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="TResult">直接结果类型。</typeparam>
        /// <param name="domainService">领域服务。</param>
        /// <param name="domainContext">领域上下文。</param>
        /// <param name="method">领域方法。</param>
        /// <returns></returns>
        public static Task<TResult> ExecuteAsync<T1, T2, T3, T4, T5, TResult>(this IDomainService domainService, IDomainContext domainContext, Func<T1, T2, T3, T4, T5, Task<TResult>> method)
        {
            if (domainService == null)
                throw new ArgumentNullException(nameof(domainService));
            return domainService.ExecuteAsync<TResult>(domainContext, method.GetMethodInfo());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="TResult">直接结果类型。</typeparam>
        /// <param name="domainService">领域服务。</param>
        /// <param name="domainContext">领域上下文。</param>
        /// <param name="method">领域方法。</param>
        /// <returns></returns>
        public static Task<TResult> ExecuteAsync<T1, T2, T3, T4, T5, T6, TResult>(this IDomainService domainService, IDomainContext domainContext, Func<T1, T2, T3, T4, T5, T6, Task<TResult>> method)
        {
            if (domainService == null)
                throw new ArgumentNullException(nameof(domainService));
            return domainService.ExecuteAsync<TResult>(domainContext, method.GetMethodInfo());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <typeparam name="TResult">直接结果类型。</typeparam>
        /// <param name="domainService">领域服务。</param>
        /// <param name="domainContext">领域上下文。</param>
        /// <param name="method">领域方法。</param>
        /// <returns></returns>
        public static Task<TResult> ExecuteAsync<T1, T2, T3, T4, T5, T6, T7, TResult>(this IDomainService domainService, IDomainContext domainContext, Func<T1, T2, T3, T4, T5, T6, T7, Task<TResult>> method)
        {
            if (domainService == null)
                throw new ArgumentNullException(nameof(domainService));
            return domainService.ExecuteAsync<TResult>(domainContext, method.GetMethodInfo());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <typeparam name="T8"></typeparam>
        /// <typeparam name="TResult">直接结果类型。</typeparam>
        /// <param name="domainService">领域服务。</param>
        /// <param name="domainContext">领域上下文。</param>
        /// <param name="method">领域方法。</param>
        /// <returns></returns>
        public static Task<TResult> ExecuteAsync<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this IDomainService domainService, IDomainContext domainContext, Func<T1, T2, T3, T4, T5, T6, T7, T8, Task<TResult>> method)
        {
            if (domainService == null)
                throw new ArgumentNullException(nameof(domainService));
            return domainService.ExecuteAsync<TResult>(domainContext, method.GetMethodInfo());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <typeparam name="T8"></typeparam>
        /// <typeparam name="T9"></typeparam>
        /// <typeparam name="TResult">直接结果类型。</typeparam>
        /// <param name="domainService">领域服务。</param>
        /// <param name="domainContext">领域上下文。</param>
        /// <param name="method">领域方法。</param>
        /// <returns></returns>
        public static Task<TResult> ExecuteAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(this IDomainService domainService, IDomainContext domainContext, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, Task<TResult>> method)
        {
            if (domainService == null)
                throw new ArgumentNullException(nameof(domainService));
            return domainService.ExecuteAsync<TResult>(domainContext, method.GetMethodInfo());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <typeparam name="T8"></typeparam>
        /// <typeparam name="T9"></typeparam>
        /// <typeparam name="T10"></typeparam>
        /// <typeparam name="TResult">直接结果类型。</typeparam>
        /// <param name="domainService">领域服务。</param>
        /// <param name="domainContext">领域上下文。</param>
        /// <param name="method">领域方法。</param>
        /// <returns></returns>
        public static Task<TResult> ExecuteAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(this IDomainService domainService, IDomainContext domainContext, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, Task<TResult>> method)
        {
            if (domainService == null)
                throw new ArgumentNullException(nameof(domainService));
            return domainService.ExecuteAsync<TResult>(domainContext, method.GetMethodInfo());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <typeparam name="T8"></typeparam>
        /// <typeparam name="T9"></typeparam>
        /// <typeparam name="T10"></typeparam>
        /// <typeparam name="T11"></typeparam>
        /// <typeparam name="TResult">直接结果类型。</typeparam>
        /// <param name="domainService">领域服务。</param>
        /// <param name="domainContext">领域上下文。</param>
        /// <param name="method">领域方法。</param>
        /// <returns></returns>
        public static Task<TResult> ExecuteAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(this IDomainService domainService, IDomainContext domainContext, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, Task<TResult>> method)
        {
            if (domainService == null)
                throw new ArgumentNullException(nameof(domainService));
            return domainService.ExecuteAsync<TResult>(domainContext, method.GetMethodInfo());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <typeparam name="T8"></typeparam>
        /// <typeparam name="T9"></typeparam>
        /// <typeparam name="T10"></typeparam>
        /// <typeparam name="T11"></typeparam>
        /// <typeparam name="T12"></typeparam>
        /// <typeparam name="TResult">直接结果类型。</typeparam>
        /// <param name="domainService">领域服务。</param>
        /// <param name="domainContext">领域上下文。</param>
        /// <param name="method">领域方法。</param>
        /// <returns></returns>
        public static Task<TResult> ExecuteAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(this IDomainService domainService, IDomainContext domainContext, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, Task<TResult>> method)
        {
            if (domainService == null)
                throw new ArgumentNullException(nameof(domainService));
            return domainService.ExecuteAsync<TResult>(domainContext, method.GetMethodInfo());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <typeparam name="T8"></typeparam>
        /// <typeparam name="T9"></typeparam>
        /// <typeparam name="T10"></typeparam>
        /// <typeparam name="T11"></typeparam>
        /// <typeparam name="T12"></typeparam>
        /// <typeparam name="T13"></typeparam>
        /// <typeparam name="TResult">直接结果类型。</typeparam>
        /// <param name="domainService">领域服务。</param>
        /// <param name="domainContext">领域上下文。</param>
        /// <param name="method">领域方法。</param>
        /// <returns></returns>
        public static Task<TResult> ExecuteAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(this IDomainService domainService, IDomainContext domainContext, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, Task<TResult>> method)
        {
            if (domainService == null)
                throw new ArgumentNullException(nameof(domainService));
            return domainService.ExecuteAsync<TResult>(domainContext, method.GetMethodInfo());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <typeparam name="T8"></typeparam>
        /// <typeparam name="T9"></typeparam>
        /// <typeparam name="T10"></typeparam>
        /// <typeparam name="T11"></typeparam>
        /// <typeparam name="T12"></typeparam>
        /// <typeparam name="T13"></typeparam>
        /// <typeparam name="T14"></typeparam>
        /// <typeparam name="TResult">直接结果类型。</typeparam>
        /// <param name="domainService">领域服务。</param>
        /// <param name="domainContext">领域上下文。</param>
        /// <param name="method">领域方法。</param>
        /// <returns></returns>
        public static Task<TResult> ExecuteAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(this IDomainService domainService, IDomainContext domainContext, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, Task<TResult>> method)
        {
            if (domainService == null)
                throw new ArgumentNullException(nameof(domainService));
            return domainService.ExecuteAsync<TResult>(domainContext, method.GetMethodInfo());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <typeparam name="T8"></typeparam>
        /// <typeparam name="T9"></typeparam>
        /// <typeparam name="T10"></typeparam>
        /// <typeparam name="T11"></typeparam>
        /// <typeparam name="T12"></typeparam>
        /// <typeparam name="T13"></typeparam>
        /// <typeparam name="T14"></typeparam>
        /// <typeparam name="T15"></typeparam>
        /// <typeparam name="TResult">直接结果类型。</typeparam>
        /// <param name="domainService">领域服务。</param>
        /// <param name="domainContext">领域上下文。</param>
        /// <param name="method">领域方法。</param>
        /// <returns></returns>
        public static Task<TResult> ExecuteAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(this IDomainService domainService, IDomainContext domainContext, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, Task<TResult>> method)
        {
            if (domainService == null)
                throw new ArgumentNullException(nameof(domainService));
            return domainService.ExecuteAsync<TResult>(domainContext, method.GetMethodInfo());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <typeparam name="T8"></typeparam>
        /// <typeparam name="T9"></typeparam>
        /// <typeparam name="T10"></typeparam>
        /// <typeparam name="T11"></typeparam>
        /// <typeparam name="T12"></typeparam>
        /// <typeparam name="T13"></typeparam>
        /// <typeparam name="T14"></typeparam>
        /// <typeparam name="T15"></typeparam>
        /// <typeparam name="T16"></typeparam>
        /// <typeparam name="TResult">直接结果类型。</typeparam>
        /// <param name="domainService">领域服务。</param>
        /// <param name="domainContext">领域上下文。</param>
        /// <param name="method">领域方法。</param>
        /// <returns></returns>
        public static Task<TResult> ExecuteAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(this IDomainService domainService, IDomainContext domainContext, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, Task<TResult>> method)
        {
            if (domainService == null)
                throw new ArgumentNullException(nameof(domainService));
            return domainService.ExecuteAsync<TResult>(domainContext, method.GetMethodInfo());
        }

        private static ConcurrentDictionary<Type, ConcurrentDictionary<string, MethodInfo>> _MethodCache = new ConcurrentDictionary<Type, ConcurrentDictionary<string, MethodInfo>>();

        /// <summary>
        /// 异步执行领域方法。
        /// </summary>
        /// <param name="domainService">领域服务。</param>
        /// <param name="domainContext">领域上下文。</param>
        /// <param name="method">方法体名称。</param>
        /// <returns>异步任务。</returns>
        public static Task ExecuteAsync(this IDomainService domainService, IDomainContext domainContext, string method)
        {
            if (domainService == null)
                throw new ArgumentNullException(nameof(domainService));
            if (domainContext == null)
                throw new ArgumentNullException(nameof(domainContext));
            if (method == null)
                throw new ArgumentNullException(nameof(method));
            var type = domainService.GetType();
            ConcurrentDictionary<string, MethodInfo> cache = _MethodCache.GetOrAdd(type, t => new ConcurrentDictionary<string, MethodInfo>());
            MethodInfo methodInfo = cache.GetOrAdd(method, t => type.GetMethod(method));
            return domainService.ExecuteAsync(domainContext, methodInfo);
        }

        /// <summary>
        /// 异步执行领域方法。
        /// </summary>
        /// <typeparam name="TResult">返回类型。</typeparam>
        /// <param name="domainService">领域服务。</param>
        /// <param name="domainContext">领域上下文。</param>
        /// <param name="method">方法体名称。</param>
        /// <returns>异步任务。</returns>
        public static Task<TResult> ExecuteAsync<TResult>(this IDomainService domainService, IDomainContext domainContext, string method)
        {
            if (domainService == null)
                throw new ArgumentNullException(nameof(domainService));
            if (domainContext == null)
                throw new ArgumentNullException(nameof(domainContext));
            if (method == null)
                throw new ArgumentNullException(nameof(method));
            var type = domainService.GetType();
            ConcurrentDictionary<string, MethodInfo> cache = _MethodCache.GetOrAdd(type, t => new ConcurrentDictionary<string, MethodInfo>());
            MethodInfo methodInfo = cache.GetOrAdd(method, t => type.GetMethod(method));
            return domainService.ExecuteAsync<TResult>(domainContext, methodInfo);
        }

        /// <summary>
        /// 异步执行领域泛型方法。
        /// </summary>
        /// <param name="domainService">领域服务。</param>
        /// <param name="domainContext">领域上下文。</param>
        /// <param name="method">方法体名称。</param>
        /// <param name="typeArguments">泛型类型参数。</param>
        /// <returns>异步任务。</returns>
        public static Task ExecuteAsync(this IDomainService domainService, IDomainContext domainContext, string method, params Type[] typeArguments)
        {
            if (domainService == null)
                throw new ArgumentNullException(nameof(domainService));
            if (domainContext == null)
                throw new ArgumentNullException(nameof(domainContext));
            if (method == null)
                throw new ArgumentNullException(nameof(method));
            var type = domainService.GetType();
            MethodInfo methodInfo = type.GetMethod(method).MakeGenericMethod(typeArguments);
            return domainService.ExecuteAsync(domainContext, methodInfo);
        }


        /// <summary>
        /// 异步执行领域泛型方法。
        /// </summary>
        /// <typeparam name="TResult">返回类型。</typeparam>
        /// <param name="domainService">领域服务。</param>
        /// <param name="domainContext">领域上下文。</param>
        /// <param name="method">方法体名称。</param>
        /// <param name="typeArguments">泛型类型参数。</param>
        /// <returns>异步任务。</returns>
        public static Task<TResult> ExecuteAsync<TResult>(this IDomainService domainService, IDomainContext domainContext, string method, params Type[] typeArguments)
        {
            if (domainService == null)
                throw new ArgumentNullException(nameof(domainService));
            if (domainContext == null)
                throw new ArgumentNullException(nameof(domainContext));
            if (method == null)
                throw new ArgumentNullException(nameof(method));
            var type = domainService.GetType();
            MethodInfo methodInfo = type.GetMethod(method).MakeGenericMethod(typeArguments);
            return domainService.ExecuteAsync<TResult>(domainContext, methodInfo);
        }
    }
}
