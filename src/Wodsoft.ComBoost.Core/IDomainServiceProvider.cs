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
        /// 覆盖领域服务。
        /// </summary>
        /// <param name="serviceType">要被覆盖的领域服务类型。</param>
        /// <param name="overrideType">要替换为的领域服务类型。</param>
        void Override(Type serviceType, Type overrideType);

        /// <summary>
        /// 注册领域扩展。
        /// </summary>
        /// <param name="serviceType">领域服务类型。</param>
        /// <param name="extensionType">领域扩展类型。</param>
        void RegisterExtension(Type serviceType, Type extensionType);

        /// <summary>
        /// 注销领域扩展。
        /// </summary>
        /// <param name="serviceType">领域服务类型。</param>
        /// <param name="extensionType">领域扩展类型。</param>
        void UnregisterExtension(Type serviceType, Type extensionType);
    }
}
