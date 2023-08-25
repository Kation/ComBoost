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

        private MockValueProvider? _valueProvider;
        public override IValueProvider ValueProvider
        {
            get
            {
                if (_valueProvider == null)
                    _valueProvider = new MockValueProvider();
                return _valueProvider;
            }
        }
    }
}
