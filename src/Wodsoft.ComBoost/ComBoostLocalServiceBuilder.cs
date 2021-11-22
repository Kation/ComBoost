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
        private static MethodInfo _GetRequiredServiceMethodInfo = typeof(ServiceProviderServiceExtensions).GetMethod("GetRequiredService", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(IServiceProvider) }, null);
        private static MethodInfo _GetContextMethodInfo = typeof(IDomainContextProvider).GetMethod("GetContext");

        public ComBoostLocalServiceBuilder(IServiceCollection services, IComBoostLocalBuilder builder)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
            LocalBuilder = builder ?? throw new ArgumentNullException(nameof(builder));

            var type = typeof(TService);
            var descriptors = type.GetCustomAttributes<DomainTemplateImplementerAttribute>();
            foreach (var descriptor in descriptors)
            {
                var descriptorType = typeof(IDomainTemplateDescriptor<>).MakeGenericType(descriptor.TemplateType);
                services.AddSingleton(descriptorType,
                    typeof(DomainTemplateBuilder<,>).MakeGenericType(typeof(TService), descriptor.TemplateType));
                var spParameter = Expression.Parameter(typeof(IServiceProvider));
                var createMethod = (Func<IServiceProvider, object>)Expression.Lambda(typeof(Func<,>).MakeGenericType(typeof(IServiceProvider), descriptor.TemplateType),
                    Expression.Call(Expression.Call(_GetRequiredServiceMethodInfo.MakeGenericMethod(descriptorType), spParameter), descriptorType.GetMethod("GetTemplate"), Expression.Call(Expression.Call(_GetRequiredServiceMethodInfo.MakeGenericMethod(typeof(IDomainContextProvider)), spParameter), _GetContextMethodInfo)),
                    spParameter).Compile();
                Services.AddTransient(descriptor.TemplateType, createMethod);
            }
            services.AddTransient<TService>();
        }

        public IServiceCollection Services { get; }

        public IComBoostLocalBuilder LocalBuilder { get; }

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
                var contextProvider = sp.GetRequiredService<IDomainContextProvider>();
                var templateDescriptor = sp.GetRequiredService<IDomainTemplateDescriptor<T>>();
                var context = contextProvider.GetContext();
                return templateDescriptor.GetTemplate(context);
            });
            return this;
        }
    }
}
