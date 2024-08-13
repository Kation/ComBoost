using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Wodsoft.ComBoost.AspNetCore;

namespace Wodsoft.ComBoost.Grpc.AspNetCore
{
    public interface IComBoostGrpcBuilder
    {
        IServiceCollection Services { get; }

        IComBoostGrpcBuilder AddTemplate<T>() where T : IDomainTemplate;

        IComBoostAspNetCoreBuilder AspNetCoreBuilder { get; }

        IComBoostGrpcBuilder UseMethodBuilder<T>()
            where T : class, IDomainGrpcMethodBuilder;
    }
}
