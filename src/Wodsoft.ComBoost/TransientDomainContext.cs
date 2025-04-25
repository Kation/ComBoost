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

        public void Dispose()
        {
            _scope.Dispose();
        }

        public object? GetService(Type serviceType)
        {
            return _scope.ServiceProvider.GetService(serviceType);
        }
    }
}
