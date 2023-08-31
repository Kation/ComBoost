using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Mock
{
    public class MockDomainContext : DomainContext
    {
        public MockDomainContext(IServiceProvider serviceProvider, CancellationToken cancellationToken)
            : base(serviceProvider, cancellationToken)
        {
            User = serviceProvider.GetRequiredService<MockAuthenticationSettings>().User;
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

        public override ClaimsPrincipal User { get; }
    }
}
