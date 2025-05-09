using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Wodsoft.ComBoost.Test
{
    public class ServiceTest
    {
        [Fact]
        public async Task GreeterServiceTest()
        {
            ServiceCollection services = new ServiceCollection();
            services.AddComBoost()
                .AddLocalService(builder =>
                {
                    builder.AddService<GreeterService>().UseTemplate<IGreeterTemplate>();
                })
                .AddMock();
            var serviceProvider = services.BuildServiceProvider();
                        
            using (var scope = serviceProvider.CreateScope())
            {
                var greeter = scope.ServiceProvider.GetRequiredService<IGreeterTemplate>();
                var request = new HelloRequest { Name = "Kation" };
                var response = await greeter.SayHi(request);
                Assert.Equal($"Hi {request.Name}.", response.Answer);

                Assert.Equal("Hi.", await greeter.Hello());
                Assert.Equal("Hi.", await greeter.Hello("I'm Kation."));

                await greeter.Test(Guid.NewGuid());
            }
        }

        [Fact]
        public async Task NotRequiredValueTest()
        {
            ServiceCollection services = new ServiceCollection();
            services.AddComBoost()
                .AddLocalService(builder =>
                {
                    builder.AddService<GreeterService>().UseTemplate<IGreeterTemplate>();
                })
                .AddMock();
            var serviceProvider = services.BuildServiceProvider();

            using (var scope = serviceProvider.CreateScope())
            {
                var greeter = scope.ServiceProvider.GetRequiredService<IGreeterTemplate>();
                await greeter.Test();
            }
        }

        [Fact]
        public async Task EventTest()
        {
            bool raised = false;

            ServiceCollection services = new ServiceCollection();
            services.AddComBoost()
                .AddLocalService(builder =>
                {
                    builder.AddService<GreeterService>()
                        .UseTemplate<IGreeterTemplate>()
                        .UseEventHandler<RequestEventArgs>((context, e) =>
                        {
                            raised = true;
                            return Task.CompletedTask;
                        });
                })
                .AddMock();
            var serviceProvider = services.BuildServiceProvider();

            using (var scope = serviceProvider.CreateScope())
            {
                var greeter = scope.ServiceProvider.GetRequiredService<IGreeterTemplate>();
                await greeter.EventTest();
            }

            Assert.True(raised);
        }

        [Fact]
        public async Task FilterTest()
        {
            ServiceCollection services = new ServiceCollection();
            services.AddComBoost()
                .AddLocalService(builder =>
                {
                    builder.AddService<GreeterService>()
                        .UseTemplate<IGreeterTemplate>()
                        .UseFilter<HelloInterruptFilter>("Hello");
                })
                .AddMock();
            var serviceProvider = services.BuildServiceProvider();

            using (var scope = serviceProvider.CreateScope())
            {
                var greeter = scope.ServiceProvider.GetRequiredService<IGreeterTemplate>();
                var request = new HelloRequest { Name = "Kation" };
                var response = await greeter.Hello();
                Assert.Equal("Interrupt", response);
            }
        }

        [Fact]
        public async Task SkipFilterTest()
        {
            ServiceCollection services = new ServiceCollection();
            services.AddComBoost()
                .AddLocalService(builder =>
                {
                    builder.AddService<GreeterService>().UseTemplate<IGreeterTemplate>();
                })
                .AddMock();
            var serviceProvider = services.BuildServiceProvider();

            using (var scope = serviceProvider.CreateScope())
            {
                var greeter = scope.ServiceProvider.GetRequiredService<IGreeterTemplate>();
                var response = await greeter.NoExecute();
                Assert.False(response);
            }
        }

        [Fact]
        public async Task LifetimeTest()
        {
            ServiceCollection services = new ServiceCollection();
            services.AddScoped<LifetimeCounter>();
            services.AddComBoost()
                .AddLocalService(builder =>
                {
                    builder.AddService<LifetimeService>();
                })
                .AddMock();
            var serviceProvider = services.BuildServiceProvider();

            using (var scope = serviceProvider.CreateScope())
            {
                var service = scope.ServiceProvider.GetRequiredService<ILifetimeService>();
                var count = await service.Increase();
                Assert.Equal(1, count);
                count = await service.Increase();
                Assert.Equal(2, count);
                count = await service.TransientIncrease();
                Assert.Equal(1, count);
                service.OverrideLifetimeStrategy = DomainLifetimeStrategy.Scope;
                count = await service.TransientIncrease();
                Assert.Equal(3, count);
                service.OverrideLifetimeStrategy = DomainLifetimeStrategy.Transient;
                count = await service.Increase();
                Assert.Equal(1, count);
            }
        }

        [Fact]
        public async Task DomainContextAccessorTest()
        {
            ServiceCollection services = new ServiceCollection();
            services.AddScoped<LifetimeCounter>();
            services.AddComBoost()
                .AddLocalService(builder =>
                {
                    builder.AddService<LifetimeService>();
                })
                .AddMock();
            var serviceProvider = services.BuildServiceProvider();

            using (var scope = serviceProvider.CreateScope())
            {
                var service = scope.ServiceProvider.GetRequiredService<ILifetimeService>();
                await service.DomainContextAccessor();
            }
        }
    }
}
