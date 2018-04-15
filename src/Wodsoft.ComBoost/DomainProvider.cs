﻿using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace Wodsoft.ComBoost
{
    public class DomainProvider : IDomainServiceProvider
    {
        private IServiceProvider _ServiceProvider;
        private Dictionary<Type, Type> _Overrides;
        private ConcurrentDictionary<Type, IDomainService> _Cache;
        private List<Func<Type, Type>> _ServiceSelectors, _ExtensionSelectors;
        private List<Func<Type, Type, bool>> _ExtensionFilters;
        private List<Func<Type, IDomainServiceFilter>> _ServiceFilterSelectors;

        public DomainProvider(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));
            _ServiceProvider = serviceProvider;
            _Overrides = new Dictionary<Type, Type>();
            _Cache = new ConcurrentDictionary<Type, IDomainService>();
            _ServiceSelectors = new List<Func<Type, Type>>();
            _ExtensionSelectors = new List<Func<Type, Type>>();
            _ExtensionFilters = new List<Func<Type, Type, bool>>();
            _ServiceFilterSelectors = new List<Func<Type, IDomainServiceFilter>>();
        }

        public virtual TService GetService<TService>() where TService : IDomainService
        {
            Type type = typeof(TService);
            IDomainService service =
            _Cache.GetOrAdd(type, ft =>
            {
                if (_Overrides.ContainsKey(type))
                    type = _Overrides[type];
                foreach (var selector in _ServiceSelectors)
                {
                    type = selector(type);
                    if (type == null)
                        throw new InvalidOperationException("领域服务选择器返回的类型为空。");
                }
                service = (TService)ActivatorUtilities.CreateInstance(_ServiceProvider, type);
                service.Initialize(_ServiceProvider);
                Type[] extensions = _ExtensionSelectors.Select(t => t(type)).Where(t => t != null).ToArray();
                foreach (var extensionType in extensions)
                {
                    if (_ExtensionFilters.Any(t => t(type, extensionType) == false))
                        continue;
                    IDomainExtension extension = (IDomainExtension)ActivatorUtilities.CreateInstance(_ServiceProvider, extensionType);
                    extension.OnInitialize(_ServiceProvider, service);
                    service.Executing += extension.OnExecutingAsync;
                    service.Executed += extension.OnExecutedAsync;
                }
                var filters = _ServiceFilterSelectors.Select(t => t(type)).Where(t => t != null).ToArray();
                foreach (var filter in filters)
                    service.Filters.Add(filter);
                return service;
            });
            return (TService)service;
        }

        public void AddService<TService, TImplementation>()
            where TService : IDomainService
            where TImplementation : TService
        {
            var serviceType = typeof(TService);
            if (serviceType.GetTypeInfo().IsGenericTypeDefinition)
                throw new NotSupportedException("不支持泛型定义类型。");
            var implementationType = typeof(TImplementation);
            if (implementationType.GetTypeInfo().IsGenericTypeDefinition)
                throw new NotSupportedException("不支持泛型定义类型。");
            if (_Overrides.ContainsKey(serviceType))
                _Overrides[serviceType] = implementationType;
            else
                _Overrides.Add(serviceType, implementationType);
        }

        public void AddServiceSelector(Func<Type, Type> serviceSelector)
        {
            if (serviceSelector == null)
                throw new ArgumentNullException(nameof(serviceSelector));
            _ServiceSelectors.Add(serviceSelector);
        }

        public void AddExtensionSelector(Func<Type, Type> extensionSelector)
        {
            if (extensionSelector == null)
                throw new ArgumentNullException(nameof(extensionSelector));
            _ExtensionSelectors.Add(extensionSelector);
        }

        public void AddExtensionFilter(Func<Type, Type, bool> extensionFilter)
        {
            if (extensionFilter == null)
                throw new ArgumentNullException(nameof(extensionFilter));
            _ExtensionFilters.Add(extensionFilter);
        }

        public void AddServiceFilterSelector(Func<Type, IDomainServiceFilter> serviceFilterSelector)
        {
            if (serviceFilterSelector == null)
                throw new ArgumentNullException(nameof(serviceFilterSelector));
            _ServiceFilterSelectors.Add(serviceFilterSelector);
        }
    }
}
