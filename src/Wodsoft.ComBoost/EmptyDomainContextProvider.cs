using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Wodsoft.ComBoost
{
    public class EmptyDomainContextProvider : IDomainContextProvider
    {
        private IServiceProvider _serviceProvider;

        public EmptyDomainContextProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public IDomainContext GetContext()
        {
            return new EmptyDomainContext(_serviceProvider, CancellationToken.None);
        }
    }
}
