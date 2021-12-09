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
