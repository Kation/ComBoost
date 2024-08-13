#if !NETCOREAPP2_1
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wodsoft.ComBoost.AspNetCore
{
    public class DomainEndpointApiDescriptorProvider<T> : IApiDescriptionProvider
    {
        private DomainEndpointApiDescriptorOptions<T> _options;

        public DomainEndpointApiDescriptorProvider(DomainEndpointApiDescriptorOptions<T> options)
        {
            _options = options;
        }

        public int Order => -100;

        public void OnProvidersExecuted(ApiDescriptionProviderContext context)
        {
            foreach (var descriptor in _options.Descriptions)
                context.Results.Add(descriptor);
        }

        public void OnProvidersExecuting(ApiDescriptionProviderContext context)
        {

        }
    }
}
#endif