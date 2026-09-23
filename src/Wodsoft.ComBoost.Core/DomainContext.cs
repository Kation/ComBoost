using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
[assembly:InternalsVisibleTo("Wodsoft.ComBoost")]

namespace Wodsoft.ComBoost
{
    public abstract class DomainContext : IDomainContext
    {
        private IServiceProvider _serviceProvider;

        public DomainContext(IServiceProvider serviceProvider, CancellationToken cancellationToken)
        {
            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));
            _serviceProvider = serviceProvider;
            Filters = new List<IDomainServiceFilter>();
            EventManager = new DomainContextEventManager(serviceProvider.GetRequiredService<IDomainServiceEventManager>());
        }

        private dynamic? _dataBag;
        public virtual dynamic DataBag
        {
            get
            {
                if (_dataBag == null)
                    _dataBag = new DomainContextDataBag();
                return _dataBag;
            }
        }

        public CancellationToken ServiceAborted { get; private set; }

        public IList<IDomainServiceFilter> Filters { get; private set; }

        public DomainServiceEventManager EventManager { get; private set; }

        public abstract IValueProvider ValueProvider { get; }

        public abstract ClaimsPrincipal User { get; }

        public IServiceProvider Services => _serviceProvider;

        public virtual object? GetService(Type serviceType)
        {
            if (serviceType == typeof(IDomainContext) || serviceType == typeof(IServiceProvider))
                return this;
            if (serviceType == typeof(IValueProvider))
                return ValueProvider;
            return _serviceProvider.GetService(serviceType);
        }

#if NET8_0_OR_GREATER

        object? IKeyedServiceProvider.GetKeyedService(Type serviceType, object? serviceKey)
        {
            if (_serviceProvider is IKeyedServiceProvider keyedServiceProvider)
                return keyedServiceProvider.GetKeyedService(serviceType, serviceKey);
            throw new NotSupportedException("Not support keyed services.");
        }

        object IKeyedServiceProvider.GetRequiredKeyedService(Type serviceType, object? serviceKey)
        {
            return _serviceProvider.GetRequiredKeyedService(serviceType, serviceKey);
        }

#endif
    }
}
