using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Wodsoft.ComBoost;
using Wodsoft.ComBoost.Data.Entity;
using Wodsoft.ComBoost.Data.Entity.Metadata;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DomainEntityFrameworkCoreExtensions
    {
        public static IServiceCollection AddEFCoreContext<TDbContext>(this IServiceCollection services, bool trackEntity = true)
            where TDbContext : DbContext
        {
            services.AddScoped(sp => new DatabaseContext<TDbContext>(sp.GetRequiredService<TDbContext>()) { TrackEntity = trackEntity });
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IHealthStateProvider, DatabaseHealthStateProvider<TDbContext>>());
            var properties = typeof(TDbContext).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(t => t.CanRead && t.CanWrite && t.PropertyType.IsConstructedGenericType && t.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>)).ToArray();
            foreach (var property in properties)
            {
                var type = property.PropertyType.GetGenericArguments()[0];
                var func = (Func<IServiceProvider, object>)Delegate.CreateDelegate(typeof(Func<IServiceProvider, object>), typeof(DatabaseContext<TDbContext>).GetMethod(nameof(DatabaseContext<TDbContext>.GetEntityContextDelegate), BindingFlags.Public | BindingFlags.Static)!.MakeGenericMethod(type));
                services.Add(new ServiceDescriptor(typeof(IEntityContext<>).MakeGenericType(type), func, ServiceLifetime.Scoped));
            }
            return services;
        }
    }
}

namespace Microsoft.EntityFrameworkCore
{
    public static class DomainEntityFrameworkExtensions
    {
        public static KeyBuilder<T> UseMetadataKeys<T>(this EntityTypeBuilder<T> builder)
            where T : class
        {
            return builder.HasKey(EntityDescriptor.GetMetadata<T>().KeyProperties.Select(t => t.ClrName).ToArray());
        }
    }
}