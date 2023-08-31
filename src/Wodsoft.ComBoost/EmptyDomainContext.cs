using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading;

namespace Wodsoft.ComBoost
{
    public sealed class EmptyDomainContext : DomainContext
    {
        public EmptyDomainContext(IServiceProvider serviceProvider, CancellationToken cancellationToken)
            : base(serviceProvider, cancellationToken)
        {
            _valueProvider = new EmptyValueProvider();
            User = new ClaimsPrincipal(new ClaimsIdentity("Anonymous"));
        }

        private EmptyValueProvider _valueProvider;
        public override IValueProvider ValueProvider => _valueProvider;

        public override ClaimsPrincipal User { get; }
    }
}
