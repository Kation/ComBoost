using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Wodsoft.ComBoost
{
    public abstract class DomainRpcContext : DomainContext
    {
        protected DomainRpcContext(IDomainRpcRequest request, IServiceProvider serviceProvider, CancellationToken cancellationToken)
            : base(serviceProvider, cancellationToken)
        {
            Request = request;
            _valueProvider = new DomainRpcValueProvider(request);
            _services = new Dictionary<Type, object>();
        }

        public IDomainRpcRequest Request { get; }

        private IValueProvider _valueProvider;
        public override IValueProvider ValueProvider => _valueProvider;

        private Dictionary<Type, object> _services;
        public override object GetService(Type serviceType)
        {
            if (_services.TryGetValue(serviceType, out var service))
                return service;
            return base.GetService(serviceType);
        }

        public void SetService<T>(T service)
        {
            if (service == null)
                throw new ArgumentNullException(nameof(service));
            _services[typeof(T)] = service;
        }
    }
}
