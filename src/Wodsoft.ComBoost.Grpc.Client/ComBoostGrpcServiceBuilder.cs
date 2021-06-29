using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost.Grpc.Client
{
    public class ComBoostGrpcServiceBuilder : IComBoostGrpcServiceBuilder
    {
        private Uri _address;
        private Func<IServiceProvider, GrpcChannelOptions> _optionsFactory;

        public ComBoostGrpcServiceBuilder(IServiceCollection services, Uri address, Func<IServiceProvider, GrpcChannelOptions> optionsFactory)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
            _address = address ?? throw new ArgumentNullException(nameof(address));
            _optionsFactory = optionsFactory ?? throw new ArgumentNullException(nameof(optionsFactory));
        }

        public IServiceCollection Services { get; }

        IComBoostGrpcServiceBuilder IComBoostGrpcServiceBuilder.UseTemplate<T>(CallOptions callOptions)
        {
            Services.AddSingleton<IDomainTemplateDescriptor<T>, GrpcTemplateBuilder<T>>(sp =>
                new GrpcTemplateBuilder<T>(GrpcChannel.ForAddress(_address, _optionsFactory(sp)), callOptions)
            );
            Services.AddTransient<T>(sp =>
            {
                var contextProvider = sp.GetRequiredService<IDomainContextProvider>();
                var templateDescriptor = sp.GetRequiredService<IDomainTemplateDescriptor<T>>();
                var context = contextProvider.GetContext();
                return templateDescriptor.GetTemplate(context);
            });
            return this;
        }
    }
}
