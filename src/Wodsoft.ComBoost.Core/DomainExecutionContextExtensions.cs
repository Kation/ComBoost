using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost
{
    /// <summary>
    /// 领域执行上下文扩展。
    /// </summary>
    public static class DomainExecutionContextExtensions
    {
        /// <summary>
        /// 获取领域服务。
        /// </summary>
        /// <typeparam name="T">领域服务类型。</typeparam>
        /// <param name="context">当前领域执行上下文。</param>
        /// <returns>获取到的领域服务。</returns>
        public static T GetDomainService<T>(this IDomainExecutionContext context)
            where T : IDomainService
        {
            var domainProvider = context.DomainContext.GetRequiredService<IDomainServiceProvider>();
            return domainProvider.GetService<T>();
        }
    }
}
