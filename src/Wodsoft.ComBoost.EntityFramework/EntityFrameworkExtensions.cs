using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Text;
using Wodsoft.ComBoost.Data.Entity;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DomainEntityFrameworkExtensions
    {
        public static IServiceCollection AddEFContext<TDbContext>(this IServiceCollection services, bool trackEntity = true)
            where TDbContext : DbContext
        {
            services.AddScoped(sp => new DatabaseContext<TDbContext>(sp.GetRequiredService<TDbContext>()) { TrackEntity = trackEntity });
            var properties = typeof(TDbContext).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(t => t.CanRead && t.CanWrite && t.PropertyType.IsConstructedGenericType && t.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>)).ToArray();
            foreach (var property in properties)
            {
                var type = property.PropertyType.GetGenericArguments()[0];
                var func = (Func<IServiceProvider, object>)Delegate.CreateDelegate(typeof(Func<IServiceProvider, object>), typeof(DatabaseContext<TDbContext>).GetMethod(nameof(DatabaseContext<TDbContext>.GetEntityContextDelegate), BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(type));
                services.Add(new ServiceDescriptor(typeof(IEntityContext<>).MakeGenericType(type), func, ServiceLifetime.Scoped));
            }
            return services;
        }
    }
}
