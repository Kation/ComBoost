using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Distributed.CAP.Test.Services;
using Wodsoft.ComBoost.Mock;
using Xunit;

namespace Wodsoft.ComBoost.Distributed.CAP.Test
{
    public class DistributedEventTest
    {
        [Fact]
        public async Task CAPTest()
        {
            var serviceMock = Mock.Mock.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddComBoost()
                        .AddLocalService(builder =>
                        {
                            builder.AddService<EventTestService>().UseTemplate<IEventTestService>();
                            builder.AddEventHandler<DomainDistributedEventPublisher<TestEventArgs>, TestEventArgs>();
                        })
                        .AddDistributed(builder =>
                        {
                            builder.UseCAP(x=>
                            {
                                x.UseInMemoryStorage();
                                x.UseRabbitMQ(options=>
                                {
                                    options.HostName = "127.0.0.1";
                                    options.UserName = "guest";
                                    options.Password = "guest";
                                });
                            });
                        })
                        .AddMock();                    
                })
                .Build();

            TaskCompletionSource taskCompletionSource = new TaskCompletionSource();

            string text = "Hello";

            var clientMock = Mock.Mock.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddComBoost()
                        //.AddLocalService(builder =>
                        //{
                        //})
                        .AddDistributed(builder =>
                        {
                            builder.AddDistributedEventHandler<TestEventArgs>((context, e) =>
                            {
                                Assert.Equal(text, e.Text);
                                taskCompletionSource.SetResult();
                                return Task.CompletedTask;
                            });
                            builder.UseCAP(x =>
                            {
                                x.UseInMemoryStorage();
                                x.UseRabbitMQ(options =>
                                {
                                    options.HostName = "127.0.0.1";
                                    options.UserName = "guest";
                                    options.Password = "guest";
                                });
                            });
                        })
                        .AddMock();
                })
                .Build();

            await clientMock.StartHostedServiceAsync();

            await serviceMock.RunAsync(async sp =>
            {
                var service = sp.GetRequiredService<IEventTestService>();
                await service.Test(text);
            });

            await taskCompletionSource.Task;

            await clientMock.StopHostedServiceAsync();
        }
    }
}
