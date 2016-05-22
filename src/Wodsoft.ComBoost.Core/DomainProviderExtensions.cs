using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    public static class DomainProviderExtensions
    {

        public static void RegisterExtension<TService, TExtension>(this IDomainProvider domainProvider)
            where TService : IDomainService
            where TExtension : IDomainExtension
        {
            if (domainProvider == null)
                throw new ArgumentNullException(nameof(domainProvider));
            domainProvider.RegisterExtension(typeof(TService), typeof(TExtension));
        }

        public static void UnregisterExtension<TService, TExtension>(this IDomainProvider domainProvider)
        {
            if (domainProvider == null)
                throw new ArgumentNullException(nameof(domainProvider));
            domainProvider.UnregisterExtension(typeof(TService), typeof(TExtension));
        }
    }
}
