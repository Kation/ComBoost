using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    /// <summary>
    /// 领域服务提供器扩展。
    /// </summary>
    public static class DomainServiceProviderExtensions
    {
        /// <summary>
        /// 注册领域扩展。
        /// </summary>
        /// <typeparam name="TService">领域服务类型。</typeparam>
        /// <typeparam name="TExtension">领域扩展类型。</typeparam>
        /// <param name="domainProvider">领域服务提供器。</param>
        public static void RegisterExtension<TService, TExtension>(this IDomainServiceProvider domainProvider)
            where TService : IDomainService
            where TExtension : IDomainExtension
        {
            if (domainProvider == null)
                throw new ArgumentNullException(nameof(domainProvider));
            domainProvider.RegisterExtension(typeof(TService), typeof(TExtension));
        }

        /// <summary>
        /// 注销领域扩展。
        /// </summary>
        /// <typeparam name="TService">领域服务类型。</typeparam>
        /// <typeparam name="TExtension">领域扩展类型。</typeparam>
        /// <param name="domainProvider">领域服务提供器。</param>
        public static void UnregisterExtension<TService, TExtension>(this IDomainServiceProvider domainProvider)
        {
            if (domainProvider == null)
                throw new ArgumentNullException(nameof(domainProvider));
            domainProvider.UnregisterExtension(typeof(TService), typeof(TExtension));
        }
    }
}
