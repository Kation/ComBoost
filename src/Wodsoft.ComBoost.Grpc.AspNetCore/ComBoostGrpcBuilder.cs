using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Wodsoft.ComBoost.AspNetCore;

namespace Wodsoft.ComBoost.Grpc.AspNetCore
{
    public class ComBoostGrpcBuilder : IComBoostGrpcBuilder
    {
        public ComBoostGrpcBuilder(IServiceCollection services, IComBoostAspNetCoreBuilder aspNetCoreBuilder)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
            AspNetCoreBuilder = aspNetCoreBuilder;
            services.AddSingleton<IDomainGrpcMethodBuilder, DomainGrpcMethodProtobufBuilder>();
        }

        public IServiceCollection Services { get; }

        public IComBoostAspNetCoreBuilder AspNetCoreBuilder { get; }

        public IComBoostGrpcBuilder AddTemplate<T>() where T : IDomainTemplate
        {
            Services.PostConfigure<DomainGrpcTemplateOptions>(options => options.AddTemplate<T>());
            return this;
        }

        public IComBoostGrpcBuilder UseMethodBuilder<T>() where T : class, IDomainGrpcMethodBuilder
        {
            Services.AddSingleton<IDomainGrpcMethodBuilder, T>();
            return this;
        }
    }
}
