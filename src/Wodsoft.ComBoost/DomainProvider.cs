using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    public class DomainProvider : IDomainProvider
    {
        private IServiceProvider _ServiceProvider;
        private Dictionary<Type, List<Type>> _Extensions;
        private Dictionary<Type, Type> _Overrides;

        public DomainProvider(IServiceProvider serviceProvider)
        {
            if (_ServiceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));
            _ServiceProvider = serviceProvider;
            _Extensions = new Dictionary<Type, List<Type>>();
            _Overrides = new Dictionary<Type, Type>();
        }

        public TService GetService<TService>() where TService : IDomainService
        {
            Type type = typeof(TService);
            TService service;
            if (type.IsConstructedGenericType)
            {
                var definitionType = type.GetGenericTypeDefinition();
                if (_Overrides.ContainsKey(definitionType))
                {
                    var arguments = type.GetGenericArguments();
                    type = _Overrides[definitionType].MakeGenericType(arguments);
                    service = (TService)ActivatorUtilities.CreateInstance(_ServiceProvider, type);
                }
                else
                    service = ActivatorUtilities.CreateInstance<TService>(_ServiceProvider);
            }
            else if (_Overrides.ContainsKey(type))
            {
                service = (TService)ActivatorUtilities.CreateInstance(_ServiceProvider, _Overrides[type]);
            }
            else
                service = ActivatorUtilities.CreateInstance<TService>(_ServiceProvider);
            List<Type> extensions;
            if (_Extensions.TryGetValue(type, out extensions))
            {
                foreach (var extensionType in extensions)
                {
                    IDomainExtension extension = (IDomainExtension)ActivatorUtilities.CreateInstance(_ServiceProvider, extensionType);
                    service.Executing += extension.OnDomainExecutingAsync;
                    service.Executed += extension.OnDomainExecutedAsync;
                }
            }
            if (type.IsConstructedGenericType)
            {
                var definitionType = type.GetGenericTypeDefinition();
                if (_Extensions.TryGetValue(definitionType, out extensions))
                {
                    var arguments = type.GetGenericArguments();
                    foreach (var extensionType in extensions)
                    {
                        IDomainExtension extension = (IDomainExtension)ActivatorUtilities.CreateInstance(_ServiceProvider, extensionType.MakeGenericType(arguments));
                        service.Executing += extension.OnDomainExecutingAsync;
                        service.Executed += extension.OnDomainExecutedAsync;
                    }
                }
            }
            return service;
        }

        public void Override(Type serviceType, Type overrideType)
        {
            if (serviceType == null)
                throw new ArgumentNullException(nameof(serviceType));
            if (overrideType == null)
                throw new ArgumentNullException(nameof(overrideType));
            if (_Overrides.ContainsKey(serviceType))
            {
                if (serviceType == overrideType)
                    _Overrides.Remove(serviceType);
                else
                    _Overrides[serviceType] = overrideType;
            }
            else
                _Overrides.Add(serviceType, overrideType);
        }

        public void RegisterExtension(Type serviceType, Type extensionType)
        {
            if (serviceType == null)
                throw new ArgumentNullException(nameof(serviceType));
            if (extensionType == null)
                throw new ArgumentNullException(nameof(extensionType));
            if (!typeof(IDomainService).IsAssignableFrom(serviceType))
                throw new ArgumentException("服务类型没有实现领域服务接口IDomainService。");
            if (!typeof(IDomainService).IsAssignableFrom(extensionType))
                throw new ArgumentException("扩展类型没有实现领域扩展接口IDomainExtension。");
            if (!_Extensions.ContainsKey(serviceType))
                _Extensions.Add(serviceType, new List<Type>());
            List<Type> extensionList = _Extensions[serviceType];
            if (!extensionList.Contains(extensionType))
                extensionList.Add(serviceType);
        }

        public void UnregisterExtension(Type serviceType, Type extensionType)
        {
            if (serviceType == null)
                throw new ArgumentNullException(nameof(serviceType));
            if (extensionType == null)
                throw new ArgumentNullException(nameof(extensionType));
            List<Type> extensionList;
            if (_Extensions.TryGetValue(serviceType, out extensionList))
            {
                extensionList.Remove(extensionType);
            }
        }
    }
}
