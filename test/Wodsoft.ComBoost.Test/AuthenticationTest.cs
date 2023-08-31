using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Mock;
using Xunit;

namespace Wodsoft.ComBoost.Test
{
    public class AuthenticationTest
    {
        [Fact]
        public async Task FromClaimsTest()
        {
            var host = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddComBoost()
                        .AddLocalService(builder =>
                        {
                            builder.AddService<AuthService>();
                        })
                        .AddMock();
                })
                .Build();

            await host.RunAsync(async sp =>
            {
                var identity = new ClaimsIdentity();
                identity.AddClaim(new Claim("name", "value"));
                sp.SetIdentity(() => identity);
                var service = sp.GetRequiredService<IAuthService>();
                var value = await service.FromClaim();
                Assert.Equal("value", value);
            });
        }

        [Fact]
        public async Task NoRoleTest()
        {
            var host = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddComBoost()
                        .AddLocalService(builder =>
                        {
                            builder.AddService<AuthService>();
                        })
                        .AddMock();
                })
                .Build();

            await host.RunAsync(async sp =>
            {
                var service = sp.GetRequiredService<IAuthService>();
                var ex = await Assert.ThrowsAsync<DomainServiceException>(service.RoleTest);
                Assert.IsType<UnauthorizedAccessException>(ex.InnerException);
            });
        }

        [Fact]
        public async Task RoleTest()
        {
            var host = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddComBoost()
                        .AddLocalService(builder =>
                        {
                            builder.AddService<AuthService>();
                        })
                        .AddMock();
                })
                .Build();

            await host.RunAsync(async sp =>
            {
                sp.SetIdentity("user", "user", "role1");
                var service = sp.GetRequiredService<IAuthService>();
                await service.RoleTest();
            });
        }
    }
}
