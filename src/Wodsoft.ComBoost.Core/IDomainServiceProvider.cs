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
        /// <param name="serviceSelector">服务选择器。传入领域服务类型，返回实际领域服务类型。</param>
        void AddServiceSelector(Func<Type, Type> serviceSelector);

        /// <summary>
        /// 添加领域扩展选择器。
        /// </summary>
        /// <param name="extensionSelector">扩展选择器。传入领域服务类型，返回领域扩展类型，可为空。</param>
        void AddExtensionSelector(Func<Type, Type> extensionSelector);

        /// <summary>
        /// 添加领域扩展筛选器。
        /// </summary>
        /// <param name="extensionFilter">扩展筛选器。传入领域服务类型，领域扩展类型，返回是否允许的布尔型结果。</param>
        void AddExtensionFilter(Func<Type, Type, bool> extensionFilter);

        /// <summary>
        /// 添加领域筛选器选择器。
        /// </summary>
        /// <param name="serviceFilterSelector">领域筛选器选择器。传入领域服务类型，返回领域服务筛选器，可为空。</param>
        void AddServiceFilterSelector(Func<Type, IDomainServiceFilter> serviceFilterSelector);
    }
}
