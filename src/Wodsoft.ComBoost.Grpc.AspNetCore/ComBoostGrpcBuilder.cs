using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost.Grpc.AspNetCore
{
    public class ComBoostGrpcBuilder : IComBoostGrpcBuilder
    {
        public ComBoostGrpcBuilder(IServiceCollection services)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
        }

        public IServiceCollection Services { get; }

        public IComBoostGrpcBuilder AddTemplate<T>() where T : IDomainTemplate
        {
            Services.PostConfigure<DomainGrpcTemplateOptions>(options => options.AddTemplate<T>());
            return this;
        }
    }
}
