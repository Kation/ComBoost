using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    /// <summary>
    /// 领域服务提供器扩展。
    /// </summary>
    public static class DomainServiceProviderExtensions
    {
        /// <summary>
        /// 添加领域泛型扩展。
        /// </summary>
        /// <param name="domainProvider">领域服务提供器。</param>
        /// <param name="serviceDefinitionType">领域服务泛型定义类型。</param>
        /// <param name="extensionDefinitionType">领域扩展泛型定义类型。</param>
        public static void AddGenericDefinitionExtension(this IDomainServiceProvider domainProvider, Type serviceDefinitionType, Type extensionDefinitionType)
        {
            if (domainProvider == null)
                throw new ArgumentNullException(nameof(domainProvider));
            if (!typeof(IDomainService).IsAssignableFrom(serviceDefinitionType))
                throw new ArgumentException("不是领域服务类型。", nameof(serviceDefinitionType));
            if (!typeof(IDomainExtension).IsAssignableFrom(extensionDefinitionType))
                throw new ArgumentException("不是领域扩展类型。", nameof(serviceDefinitionType));
            if (!serviceDefinitionType.GetTypeInfo().IsGenericTypeDefinition)
                throw new ArgumentException("仅支持泛型定义类型。。", nameof(serviceDefinitionType));
            if (!extensionDefinitionType.GetTypeInfo().IsGenericTypeDefinition)
                throw new ArgumentException("仅支持泛型定义类型。。", nameof(extensionDefinitionType));
            var serviceConstraints = serviceDefinitionType.GetTypeInfo().GenericTypeParameters;
            var extensionConstraints = extensionDefinitionType.GetTypeInfo().GenericTypeParameters;
            if (serviceConstraints.Length != extensionConstraints.Length)
                throw new ArgumentException("领域服务类型与领域扩展类型的泛型数量不一致。");
            //for (int i = 0; i < serviceConstraints.Length; i++)
            //{
            //    if (extensionConstraints[i].GetTypeInfo().GetGenericParameterConstraints().Any(t => !t.IsAssignableFrom(serviceConstraints[i])))
            //        throw new ArgumentException("领域服务第" + i + "个泛型约束不满足领域扩展泛型约束。");
            //}
            domainProvider.AddExtensionSelector(type =>
            {
                if (!type.GetTypeInfo().IsGenericType)
                    return null;
                if (type.GetGenericTypeDefinition() != serviceDefinitionType)
                    return null;
                try
                {
                    var extensionType = extensionDefinitionType.MakeGenericType(type.GetGenericArguments());
                    return extensionType;
                }
                catch
                {
                    return null;
                }
            });
        }

        /// <summary>
        /// 添加领域扩展。
        /// </summary>
        /// <param name="domainProvider">领域服务提供器。</param>
        /// <param name="serviceType">领域服务类型。（支持子类绑定。）</param>
        /// <param name="extensionType">领域扩展类型。</param>
        public static void AddExtension(this IDomainServiceProvider domainProvider, Type serviceType, Type extensionType)
        {
            if (domainProvider == null)
                throw new ArgumentNullException(nameof(domainProvider));
            if (!typeof(IDomainService).IsAssignableFrom(serviceType))
                throw new ArgumentException("不是领域服务类型。", nameof(serviceType));
            if (!typeof(IDomainExtension).IsAssignableFrom(extensionType))
                throw new ArgumentException("不是领域扩展类型。", nameof(serviceType));
            if (serviceType.GetTypeInfo().IsGenericTypeDefinition)
                throw new ArgumentException("不支持泛型定义类型。。", nameof(serviceType));
            if (extensionType.GetTypeInfo().IsGenericTypeDefinition)
                throw new ArgumentException("不支持泛型定义类型。。", nameof(extensionType));
            domainProvider.AddExtensionSelector(t => serviceType.IsAssignableFrom(t) ? extensionType : null);
        }

        /// <summary>
        /// 添加领域扩展。
        /// </summary>
        /// <typeparam name="TService">领域服务类型。（支持子类绑定。）</typeparam>
        /// <typeparam name="TExtension">领域扩展类型。</typeparam>
        /// <param name="domainProvider">领域服务提供器。</param>
        public static void AddExtension<TService, TExtension>(this IDomainServiceProvider domainProvider)
            where TService : IDomainService
            where TExtension : IDomainExtension
        {
            AddExtension(domainProvider, typeof(TService), typeof(TExtension));
        }

        /// <summary>
        /// 移除领域扩展。（最终判定）
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TExtension"></typeparam>
        /// <param name="domainProvider"></param>
        public static void RemoveExtension<TService, TExtension>(this IDomainServiceProvider domainProvider)
            where TService : IDomainService
            where TExtension : IDomainExtension
        {
            if (domainProvider == null)
                throw new ArgumentNullException(nameof(domainProvider));
            domainProvider.AddExtensionFilter((service, extension) => service == typeof(TService) && extension == typeof(TExtension) ? false : true);
        }

        /// <summary>
        /// 添加全局领域服务过滤器。
        /// </summary>
        /// <typeparam name="TFilter">过滤器类型。</typeparam>
        /// <param name="domainProvider">领域服务提供器。</param>
        public static void AddGlobalFilter<TFilter>(this IDomainServiceProvider domainProvider)
            where TFilter : class, IDomainServiceFilter, new()
        {
            domainProvider.AddServiceFilterSelector(t => new TFilter());
        }

        /// <summary>
        /// 添加领域服务过滤器。
        /// </summary>
        /// <typeparam name="TService">领域服务类型。</typeparam>
        /// <typeparam name="TFilter">过滤器类型。</typeparam>
        /// <param name="domainProvider">领域服务提供器。</param>
        public static void AddServiceFilter<TService, TFilter>(this IDomainServiceProvider domainProvider)
            where TService : IDomainService
            where TFilter : class, IDomainServiceFilter, new()
        {
            domainProvider.AddServiceFilterSelector(t => typeof(TService).IsAssignableFrom(t) ? new TFilter() : null);
        }
    }
}
