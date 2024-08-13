using Grpc.Net.Client;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost.Grpc.Client
{
    public class ComBoostGrpcBuilder : IComBoostGrpcBuilder
    {
        private IDomainGrpcCallOptionsHandler? _callOptionsHandler;

        public ComBoostGrpcBuilder(IServiceCollection services, IComBoostBuilder comBoostBuilder)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
            ComBoostBuilder = comBoostBuilder;
        }

        public IServiceCollection Services { get; }

        public IComBoostBuilder ComBoostBuilder { get; }

        public IComBoostGrpcServiceBuilder AddService(Uri address, Func<IServiceProvider, GrpcChannelOptions> optionsFactory)
        {
            ComBoostGrpcServiceBuilder builder = new ComBoostGrpcServiceBuilder(Services, address ?? throw new ArgumentNullException(nameof(address)), optionsFactory, _callOptionsHandler);
            return builder;
        }

        public IComBoostGrpcBuilder UseCallOptionsHandler(IDomainGrpcCallOptionsHandler handler)
        {
            _callOptionsHandler = handler;
            return this;
        }
    }
}
