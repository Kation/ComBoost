using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Mock;
using Wodsoft.ComBoost;

namespace DataUnitTest
{
    public class EventTestEnvironment : MockEnvironment
    {
        private string _DatabaseName = Guid.NewGuid().ToString();

        protected override void ConfigureDomainService(IDomainServiceProvider domainProvider)
        {
            domainProvider.AddExtension<EventDomainService, EventDomainExtension>();
        }
    }
}
