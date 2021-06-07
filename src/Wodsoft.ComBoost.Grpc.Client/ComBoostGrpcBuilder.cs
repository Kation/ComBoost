using Grpc.Net.Client;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost.Grpc.Client
{
    public class ComBoostGrpcBuilder : IComBoostGrpcBuilder
    {
        public ComBoostGrpcBuilder(IServiceCollection services)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
        }

        public IServiceCollection Services { get; }

        public IComBoostGrpcServiceBuilder AddService(Uri address, Func<IServiceProvider, GrpcChannelOptions> optionsFactory)
        {
            ComBoostGrpcServiceBuilder builder = new ComBoostGrpcServiceBuilder(Services, address ?? throw new ArgumentNullException(nameof(address)), optionsFactory);
            return builder;
        }

    }
}
