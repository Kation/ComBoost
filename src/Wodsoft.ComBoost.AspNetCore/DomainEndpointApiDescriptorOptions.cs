#if !NETCOREAPP2_1
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost.AspNetCore
{
    public class DomainEndpointApiDescriptorOptions
    {
        public List<ApiDescription> Descriptions { get; } = new List<ApiDescription>();
    }

    public class DomainEndpointApiDescriptorOptions<T> : DomainEndpointApiDescriptorOptions
    {

    }
}
#endif