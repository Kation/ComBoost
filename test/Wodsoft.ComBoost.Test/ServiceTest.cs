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
                .AddEmptyContextProvider();
            var serviceProvider = services.BuildServiceProvider();

            using (var scope = serviceProvider.CreateScope())
            {
                var greeter = scope.ServiceProvider.GetRequiredService<IGreeterTemplate>();
                var request = new HelloRequest { Name = "Kation" };
                var response = await greeter.SayHi(request);
                Assert.Equal($"Hi {request.Name}.", response.Answer);

                await greeter.Hello();
                await greeter.Hello("I'm Kation.");
            }
        }
    }
}
