using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost.Grpc.AspNetCore
{
    public class DomainGrpcDiscoveryService
    {
        public DomainGrpcDiscoveryService(IServiceProvider services)
        {
            Services = services;
        }

        public IServiceProvider Services { get; }
    }
}
