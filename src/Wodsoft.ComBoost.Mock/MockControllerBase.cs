using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Mock
{
    public abstract class MockControllerBase
    {
        public MockControllerBase(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));
            ServiceProvider = serviceProvider;
            DomainProvider = serviceProvider.GetRequiredService<IDomainServiceProvider>();
        }

        public IServiceProvider ServiceProvider { get; private set; }

        public IDomainServiceProvider DomainProvider { get; private set; }

        public abstract MockDomainContext CreateDomainContext();
    }
}
