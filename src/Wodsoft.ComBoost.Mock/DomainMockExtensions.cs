﻿using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Mock
{
    public static class DomainMockExtensions
    {
        public static async Task RunAsync(this IHost host, Func<IServiceProvider, Task> action)
        {
            using (var scope = host.Services.CreateScope())
            {
                await action(scope.ServiceProvider);
            }
        }

        public static void Run(this IHost host, Action<IServiceProvider> action)
        {
            using (var scope = host.Services.CreateScope())
            {
                action(scope.ServiceProvider);
            }
        }

        public static Task RunAsDomainMethodAsync(this IHost host, Func<IDomainExecutionContext, Task> action)
        {
            using (var scope = host.Services.CreateScope())
            {
                var context = new DomainDistributedExecutionContext(new EmptyDomainContext(scope.ServiceProvider, default));
                return action(context);
            }
        }

        public static Task RunAsDomainMethodAsync<TService>(this IHost host, Func<IDomainExecutionContext, TService, Task> action)
        {
            using (var scope = host.Services.CreateScope())
            {
                var context = new DomainDistributedExecutionContext(new EmptyDomainContext(scope.ServiceProvider, default));
                return action(context, scope.ServiceProvider.GetRequiredService<TService>());
            }
        }

        public static Task RunAsDomainMethodAsync<TService1, TService2>(this IHost host, Func<IDomainExecutionContext, TService1, TService2, Task> action)
        {
            using (var scope = host.Services.CreateScope())
            {
                var context = new DomainDistributedExecutionContext(new EmptyDomainContext(scope.ServiceProvider, default));
                return action(context,
                    scope.ServiceProvider.GetRequiredService<TService1>(),
                    scope.ServiceProvider.GetRequiredService<TService2>());
            }
        }

        public static Task RunAsDomainMethodAsync<TService1, TService2, TService3>(this IHost host, Func<IDomainExecutionContext, TService1, TService2, TService3, Task> action)
        {
            using (var scope = host.Services.CreateScope())
            {
                var context = new DomainDistributedExecutionContext(new EmptyDomainContext(scope.ServiceProvider, default));
                return action(context,
                    scope.ServiceProvider.GetRequiredService<TService1>(),
                    scope.ServiceProvider.GetRequiredService<TService2>(),
                    scope.ServiceProvider.GetRequiredService<TService3>());
            }
        }

        public static Task RunAsDomainMethodAsync<TService1, TService2, TService3, TService4>(this IHost host, Func<IDomainExecutionContext, TService1, TService2, TService3, TService4, Task> action)
        {
            using (var scope = host.Services.CreateScope())
            {
                var context = new DomainDistributedExecutionContext(new EmptyDomainContext(scope.ServiceProvider, default));
                return action(context,
                    scope.ServiceProvider.GetRequiredService<TService1>(),
                    scope.ServiceProvider.GetRequiredService<TService2>(),
                    scope.ServiceProvider.GetRequiredService<TService3>(),
                    scope.ServiceProvider.GetRequiredService<TService4>());
            }
        }

        public static Task RaiseEvent<T>(this IHost host, T args)
            where T : DomainServiceEventArgs
        {
            return RunAsDomainMethodAsync(host, context => context.DomainContext.EventManager.RaiseEvent(context, args));
        }

        public static void SetIdentity(this IServiceProvider services, Func<ClaimsIdentity> identityBuilder)
        {
            if (identityBuilder == null)
                throw new ArgumentNullException(nameof(identityBuilder));
            var settings = services.GetRequiredService<MockAuthenticationSettings>();
            settings.User.AddIdentity(identityBuilder());
        }

        public static void SetIdentity(this IServiceProvider services, string userId, string userName, params string[] roles)
        {
            SetIdentity(services, () =>
            {
                var identity = new ClaimsIdentity("Mock", ClaimTypes.Name, ClaimTypes.Role);
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, userId ?? throw new ArgumentNullException(nameof(userId))));
                identity.AddClaim(new Claim(ClaimTypes.Name, userName ?? throw new ArgumentNullException(nameof(userName))));
                foreach (var role in roles)
                    identity.AddClaim(new Claim(ClaimTypes.Role, role));
                return identity;
            });
        }
    }
}
