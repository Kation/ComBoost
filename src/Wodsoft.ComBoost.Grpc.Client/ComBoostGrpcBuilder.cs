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
        private IDomainGrpcMethodBuilder _methodBuilder;

        public ComBoostGrpcBuilder(IServiceCollection services, IComBoostBuilder comBoostBuilder)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
            ComBoostBuilder = comBoostBuilder;
            _methodBuilder = new DomainGrpcMethodProtobufBuilder();
        }

        public IServiceCollection Services { get; }

        public IComBoostBuilder ComBoostBuilder { get; }

        public IComBoostGrpcServiceBuilder AddService(Uri address, Func<IServiceProvider, GrpcChannelOptions> optionsFactory)
        {
            ComBoostGrpcServiceBuilder builder = new ComBoostGrpcServiceBuilder(Services, address ?? throw new ArgumentNullException(nameof(address)), optionsFactory, _callOptionsHandler, _methodBuilder);
            return builder;
        }

        public IComBoostGrpcBuilder UseCallOptionsHandler(IDomainGrpcCallOptionsHandler handler)
        {
            _callOptionsHandler = handler;
            return this;
        }

        public IComBoostGrpcBuilder UseMethodBuilder(IDomainGrpcMethodBuilder methodBuilder)
        {
            _methodBuilder = methodBuilder ?? throw new ArgumentNullException(nameof(methodBuilder));
            return this;
        }
    }
}
