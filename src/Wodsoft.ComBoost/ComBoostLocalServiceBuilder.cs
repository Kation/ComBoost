using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Wodsoft.ComBoost
{
    public class ComBoostLocalServiceBuilder<TService> : IComBoostLocalServiceBuilder<TService>
        where TService : class, IDomainService
    {
        private static MethodInfo _UseTemplateMethod = typeof(ComBoostLocalServiceBuilder<TService>).GetMethod("UseTemplate")!;

        public ComBoostLocalServiceBuilder(IServiceCollection services, IComBoostLocalBuilder builder)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
            LocalBuilder = builder ?? throw new ArgumentNullException(nameof(builder));

            var type = typeof(TService);
            var descriptors = type.GetCustomAttributes<DomainTemplateImplementerAttribute>();
            foreach (var descriptor in descriptors)
                _UseTemplateMethod.MakeGenericMethod(descriptor.TemplateType).Invoke(this, null);
            services.AddTransient<TService>();
        }

        public IServiceCollection Services { get; }

        public IComBoostLocalBuilder LocalBuilder { get; }

        public IComBoostLocalServiceBuilder<TService> UseEventHandler<TArgs>(DomainServiceEventHandler<TArgs> handler) where TArgs : DomainServiceEventArgs
        {
            Services.PostConfigure<DomainServiceEventManagerOptions<TService>>(options => options.AddEventHandler(handler));
            return this;
        }

        public IComBoostLocalServiceBuilder<TService> UseEventHandler<THandler, TArgs>()
            where THandler : IDomainServiceEventHandler<TArgs>, new()
            where TArgs : DomainServiceEventArgs
        {
            Services.PostConfigure<DomainServiceEventManagerOptions<TService>>(options =>
            {
                var handler = new THandler();
                options.AddEventHandler<TArgs>(handler.Handle);
            });
            return this;
        }

        public IComBoostLocalServiceBuilder<TService> UseFilter<T>(params string[] methods)
            where T : class, IDomainServiceFilter, new()
        {
            if (methods.Length == 0)
                Services.PostConfigure<DomainFilterOptions<TService>>(options =>
                {
                    options.Add(new T());
                });
            else
                foreach (var method in methods)
                    Services.PostConfigure<DomainFilterOptions<TService>>(method, options =>
                    {
                        options.Add(new T());
                    });
            return this;
        }

        public IComBoostLocalServiceBuilder<TService> UseTemplate<T>()
            where T : class, IDomainTemplate
        {
            Services.AddSingleton<IDomainTemplateDescriptor<T>, DomainTemplateBuilder<TService, T>>();
            Services.AddTransient<T>(sp =>
            {
                var contextAccessor = sp.GetRequiredService<IDomainContextAccessor>();
                var templateDescriptor = sp.GetRequiredService<IDomainTemplateDescriptor<T>>();
                var context = contextAccessor.Context;
                if (context == null)
                    context = sp.GetRequiredService<IDomainContextProvider>().GetContext();
                return templateDescriptor.GetTemplate(context);
            });
            return this;
        }
    }
}
