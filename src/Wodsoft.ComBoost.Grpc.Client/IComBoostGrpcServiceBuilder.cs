using Grpc.Core;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost.Grpc.Client
{
    public interface IComBoostGrpcServiceBuilder
    {
        IServiceCollection Services { get; }

        IComBoostGrpcServiceBuilder UseTemplate<T>(CallOptions callOptions = default(CallOptions)) where T : class, IDomainTemplate;
    }
}
