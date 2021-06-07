using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Wodsoft.ComBoost
{
    public sealed class EmptyDomainContext : DomainContext
    {
        public EmptyDomainContext(IServiceProvider serviceProvider, CancellationToken cancellationToken)
            : base(serviceProvider, cancellationToken)
        {
            ValueProvider = new EmptyValueProvider();
        }
        
        public EmptyValueProvider ValueProvider { get; private set; }

        public override object GetService(Type serviceType)
        {
            if (serviceType == typeof(IValueProvider) || serviceType == typeof(IConfigurableValueProvider))
                return ValueProvider;
            return base.GetService(serviceType);
        }
    }
}
