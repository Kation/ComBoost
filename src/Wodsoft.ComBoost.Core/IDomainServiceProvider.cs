using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    /// <summary>
    /// 领域提供器接口。
    /// </summary>
    public interface IDomainServiceProvider
    {
        /// <summary>
        /// 获取领域服务。
        /// </summary>
        /// <typeparam name="TService">领域服务类型。</typeparam>
        /// <returns>返回领域服务。</returns>
        TService GetService<TService>() where TService : IDomainService;

        /// <summary>
        /// 添加领域服务。
        /// </summary>
        /// <typeparam name="TService">目标服务类型。</typeparam>
        /// <typeparam name="TImplementation">实现服务类型。</typeparam>
        void AddService<TService, TImplementation>()
            where TService : IDomainService
            where TImplementation : TService;

        /// <summary>
        /// 添加领域服务选择器。
        /// </summary>
        /// <param name="serviceSelector">服务选择器。</param>
        void AddServiceSelector(Func<Type, Type> serviceSelector);

        /// <summary>
        /// 添加领域扩展选择器。
        /// </summary>
        /// <param name="extensionSelector">扩展选择器。</param>
        void AddExtensionSelector(Func<Type, Type> extensionSelector);
    }
}
