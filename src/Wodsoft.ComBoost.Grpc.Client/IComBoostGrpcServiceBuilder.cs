using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost.Grpc.Client
{
    public interface IComBoostGrpcServiceBuilder
    {
        IServiceCollection Services { get; }

        IComBoostGrpcServiceBuilder UseTemplate<T>() where T : class, IDomainTemplate;
    }
}
