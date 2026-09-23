using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading;

namespace Wodsoft.ComBoost
{
    public class TransientDomainContext : IDomainContext, IDisposable
    {
        private readonly IDomainContext _innerContext;
        private readonly IServiceScope _scope;

        public TransientDomainContext(IDomainContext domainContext)
        {
            _innerContext = domainContext;
            _scope = domainContext.GetRequiredService<IServiceScopeFactory>().CreateScope();
        }

        public CancellationToken ServiceAborted => _innerContext.ServiceAborted;

        public dynamic DataBag => _innerContext.DataBag;

        public IList<IDomainServiceFilter> Filters => _innerContext.Filters;

        public DomainServiceEventManager EventManager => _innerContext.EventManager;

        public IValueProvider ValueProvider => _innerContext.ValueProvider;

        public ClaimsPrincipal User => _innerContext.User;

        public IServiceProvider Services => _scope.ServiceProvider;

        public void Dispose()
        {
            _scope.Dispose();
        }

        public object? GetService(Type serviceType)
        {
            if (serviceType == typeof(IDomainContext) || serviceType == typeof(IServiceProvider))
                return this;
            if (serviceType == typeof(IValueProvider))
                return _innerContext.ValueProvider;
            return _scope.ServiceProvider.GetService(serviceType);
        }

#if NET8_0_OR_GREATER

        object? IKeyedServiceProvider.GetKeyedService(Type serviceType, object? serviceKey)
        {
            if (_scope.ServiceProvider is IKeyedServiceProvider keyedServiceProvider)
                return keyedServiceProvider.GetKeyedService(serviceType, serviceKey);
            throw new NotSupportedException("Not support keyed services.");
        }

        object IKeyedServiceProvider.GetRequiredKeyedService(Type serviceType, object? serviceKey)
        {
            return _scope.ServiceProvider.GetRequiredKeyedService(serviceType, serviceKey);
        }

#endif
    }
}
