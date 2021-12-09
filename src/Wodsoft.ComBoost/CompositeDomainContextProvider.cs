using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost
{
    public class CompositeDomainContextProvider : IDomainContextProvider
    {
        private readonly IReadOnlyList<Type> _providers;
        private readonly IServiceProvider _serviceProvider;

        public CompositeDomainContextProvider(IServiceProvider serviceProvider, IOptions<CompositeDomainContextProviderOptions> options)
        {
            _serviceProvider = serviceProvider;
            _providers = options.Value.Providers;
        }

        public bool CanProvide => _providers.Count > 0;

        public IDomainContext GetContext()
        {
            for (int i = 0; i < _providers.Count; i++)
            {
                var type = _providers[i];
                IDomainContextProvider provider = (IDomainContextProvider)ActivatorUtilities.CreateInstance(_serviceProvider, type);
                if (provider.CanProvide)
                    return provider.GetContext();
            }
            throw new NotSupportedException("Currently providers can not provide any domain context.");
        }
    }
}
