using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    public class DomainProvider : IDomainServiceProvider
    {
        private IServiceProvider _ServiceProvider;
        private Dictionary<Type, Type> _Overrides;
        private Dictionary<Type, IDomainService> _Cache;
        private List<Func<Type, Type>> _ServiceSelectors, _ExtensionSelectors;

        public DomainProvider(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));
            _ServiceProvider = serviceProvider;
            _Overrides = new Dictionary<Type, Type>();
            _Cache = new Dictionary<Type, IDomainService>();
            _ServiceSelectors = new List<Func<Type, Type>>();
            _ExtensionSelectors = new List<Func<Type, Type>>();
        }

        public virtual TService GetService<TService>() where TService : IDomainService
        {
            Type type = typeof(TService);
            IDomainService service;
            if (_Cache.TryGetValue(type, out service))
            {
                return (TService)service;
            }
            if (_Overrides.ContainsKey(type))
                type = _Overrides[type];
            foreach (var selector in _ServiceSelectors)
            {
                type = selector(type);
                if (type == null)
                    throw new InvalidOperationException("领域服务选择器返回的类型为空。");
            }
            service = (TService)ActivatorUtilities.CreateInstance(_ServiceProvider, type);
            Type[] extensions = _ExtensionSelectors.Select(t => t(type)).Where(t => t != null).ToArray();
            foreach (var extensionType in extensions)
            {
                IDomainExtension extension = (IDomainExtension)ActivatorUtilities.CreateInstance(_ServiceProvider, extensionType);
                extension.OnInitialize(_ServiceProvider, service);
                service.Executing += extension.OnExecutingAsync;
                service.Executed += extension.OnExecutedAsync;
            }
            _Cache.Add(type, service);
            return (TService)service;
        }

        public void AddService<TService, TImplementation>()
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
    }
}
