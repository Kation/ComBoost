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
        private IDomainGrpcCallOptionsHandler? _callOptionsHandler;
        private IDomainGrpcMethodBuilder _methodBuilder;

        public ComBoostGrpcServiceBuilder(IServiceCollection services, Uri address, Func<IServiceProvider, GrpcChannelOptions> optionsFactory, IDomainGrpcCallOptionsHandler? callOptionsHandler, IDomainGrpcMethodBuilder methodBuilder)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
            _address = address ?? throw new ArgumentNullException(nameof(address));
            _optionsFactory = optionsFactory ?? throw new ArgumentNullException(nameof(optionsFactory));
            _callOptionsHandler = callOptionsHandler;
            _methodBuilder = methodBuilder;
        }

        public IServiceCollection Services { get; }

        public Uri Address => _address;

        public Func<IServiceProvider, GrpcChannelOptions> OptionsFactory => _optionsFactory;

        public IComBoostGrpcServiceBuilder UseCallOptionsHandler(IDomainGrpcCallOptionsHandler handler)
        {
            _callOptionsHandler = handler;
            return this;
        }

        public IComBoostGrpcServiceBuilder UseMethodBuilder(IDomainGrpcMethodBuilder methodBuilder)
        {
            _methodBuilder = methodBuilder ?? throw new ArgumentNullException(nameof(methodBuilder));
            return this;
        }

        public IComBoostGrpcServiceBuilder UseTemplate<T>(CallOptions callOptions = default)
             where T : class, IDomainTemplate
        {
            if (_callOptionsHandler != null)
                _callOptionsHandler.Handle(typeof(T), ref callOptions);
            Services.AddSingleton<IDomainTemplateDescriptor<T>, GrpcTemplateBuilder<T>>(sp =>
            {
                GrpcTemplateBuilder<T>.Build(_methodBuilder);
                return new GrpcTemplateBuilder<T>(GrpcChannel.ForAddress(_address, _optionsFactory(sp)), callOptions);
            });
            Services.AddTransient<T>(sp =>
            {
                var contextAccessor = sp.GetRequiredService<IDomainContextAccessor>();
                var templateDescriptor = sp.GetRequiredService<IDomainTemplateDescriptor<T>>();
                var context = contextAccessor.Context;
                return templateDescriptor.GetTemplate(context);
            });
            return this;
        }
    }
}
