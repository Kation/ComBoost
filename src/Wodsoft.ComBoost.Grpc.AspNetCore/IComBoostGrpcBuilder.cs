using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost.Grpc.AspNetCore
{
    public interface IComBoostGrpcBuilder
    {
        IServiceCollection Services { get; }

        IComBoostGrpcBuilder AddTemplate<T>() where T : IDomainTemplate;
    }
}
