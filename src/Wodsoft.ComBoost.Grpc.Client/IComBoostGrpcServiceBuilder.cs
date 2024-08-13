using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost.Grpc.Client
{
    public interface IComBoostGrpcServiceBuilder
    {
        IServiceCollection Services { get; }

        Uri Address { get; }

        Func<IServiceProvider, GrpcChannelOptions> OptionsFactory { get; }

        IComBoostGrpcServiceBuilder UseCallOptionsHandler(IDomainGrpcCallOptionsHandler handler);

        IComBoostGrpcServiceBuilder UseTemplate<T>(CallOptions callOptions = default(CallOptions)) where T : class, IDomainTemplate;
    }
}
