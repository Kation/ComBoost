﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    /// <summary>
    /// 领域服务。
    /// </summary>
    public interface IDomainService
    {
        /// <summary>
        /// 获取领域执行上下文。
        /// </summary>
        IDomainExecutionContext Context { get; }

        /// <summary>
        /// 获取领域服务过滤器列表。
        /// </summary>
        IList<IDomainServiceFilter> Filters { get; }

        /// <summary>
        /// 调用执行领域方法。
        /// </summary>
        /// <param name="domainContext">领域上下文。</param>
        /// <param name="method">执行方法信息。</param>
        /// <returns>返回异步任务。</returns>
        Task ExecuteAsync(IDomainContext domainContext, MethodInfo method);
        /// <summary>
        /// 调用执行领域方法。
        /// </summary>
        /// <typeparam name="T">方法返回类型。</typeparam>
        /// <param name="domainContext">领域上下文。</param>
        /// <param name="method">执行方法信息。</param>
        /// <returns>返回异步任务。</returns>
        Task<T> ExecuteAsync<T>(IDomainContext domainContext, MethodInfo method);

        /// <summary>
        /// 领域方法执行前事件。
        /// </summary>
        event DomainServiceAsyncEventHandler<DomainServiceEventArgs> Executing;

        /// <summary>
        /// 领域方法执行后事件。
        /// </summary>
        event DomainServiceAsyncEventHandler<DomainServiceEventArgs> Executed;

        /// <summary>
        /// 初始化领域服务。
        /// </summary>
        /// <param name="serviceProvider"></param>
        void Initialize(IServiceProvider serviceProvider);
    }
}
