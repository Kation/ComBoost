using Grpc.Net.Client;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost.Grpc.Client
{
    public interface IComBoostGrpcBuilder
    {
        IServiceCollection Services { get; }

        IComBoostGrpcServiceBuilder AddService(Uri address, Func<IServiceProvider, GrpcChannelOptions> optionsFactory);

        IComBoostBuilder ComBoostBuilder { get; }

        IComBoostGrpcBuilder UseCallOptionsHandler(IDomainGrpcCallOptionsHandler handler);

        IComBoostGrpcBuilder UseMethodBuilder(IDomainGrpcMethodBuilder methodBuilder);
    }
}
