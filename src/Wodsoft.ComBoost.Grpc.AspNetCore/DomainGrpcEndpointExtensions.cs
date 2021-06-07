using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Wodsoft.ComBoost;
using Wodsoft.ComBoost.Grpc.AspNetCore;

namespace Microsoft.AspNetCore.Builder
{
    public static class DomainGrpcEndpointExtensions
    {
        public static void MapDomainGrpcService(this IEndpointRouteBuilder endpoint)
        {
            if (endpoint == null)
                throw new ArgumentNullException(nameof(endpoint));
            endpoint.MapGrpcService<DomainGrpcDiscoveryService>();
        }
    }
}
