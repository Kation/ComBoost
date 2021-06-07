using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Mock
{
    public class MockDomainContext : DomainContext
    {
        public MockDomainContext(IServiceProvider serviceProvider, CancellationToken cancellationToken)
            : base(serviceProvider, cancellationToken)
        {

        }

        private MockValueProvider _ValueProvider;
        public MockValueProvider ValueProvider
        {
            get
            {
                if (_ValueProvider == null)
                    _ValueProvider = new MockValueProvider();
                return _ValueProvider;
            }
        }

        public override object GetService(Type serviceType)
        {
            if (serviceType == typeof(IValueProvider) || serviceType == typeof(IConfigurableValueProvider))
                return ValueProvider;
            return base.GetService(serviceType);
        }
    }
}
