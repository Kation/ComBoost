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
            _valueProvider = new DomainRpcValueProvider(request);
        }

        private DomainRpcValueProvider _valueProvider;
        public override object GetService(Type serviceType)
        {
            if (serviceType == typeof(IValueProvider) || serviceType == typeof(IConfigurableValueProvider))
                return _valueProvider;
            return base.GetService(serviceType);
        }
    }
}
