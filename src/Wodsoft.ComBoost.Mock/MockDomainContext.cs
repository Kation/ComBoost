using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Mock
{
    public class MockDomainContext : DomainContext
    {
        public MockDomainContext(IServiceProvider serviceProvider, CancellationTokenSource cancellationTokenSource)
            : base(serviceProvider, cancellationTokenSource.Token)
        {
            CancellationTokenSource = cancellationTokenSource;
        }

        public CancellationTokenSource CancellationTokenSource { get; private set; }

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
            if (serviceType == typeof(IValueProvider))
                return ValueProvider;
            return base.GetService(serviceType);
        }
    }
}
