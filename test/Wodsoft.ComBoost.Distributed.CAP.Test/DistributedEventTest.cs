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
        volatile int _handleOnceCount = 0;
        volatile int _handleGroupCount = 0;

        [Fact]
        public async Task HandleOnceTest()
        {
            var serviceMock = Mock.Mock.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddComBoost()
                        .AddLocalService(builder =>
                        {
                            builder.AddService<EventTestService>().UseTemplate<IEventTestService>();
                            builder.AddEventHandler<DomainDistributedEventPublisher<HandleOnceEventArgs>, HandleOnceEventArgs>();
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

            string text = "Hello";

            var client1Mock = Mock.Mock.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddComBoost()
                        .AddDistributed(builder =>
                        {
                            builder.AddDistributedEventHandler<HandleOnceEventArgs>((context, e) =>
                            {
                                Assert.Equal(text, e.Text);
                                _handleOnceCount++;
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

            var client2Mock = Mock.Mock.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddComBoost()
                        .AddDistributed(builder =>
                        {
                            builder.AddDistributedEventHandler<HandleOnceEventArgs>((context, e) =>
                            {
                                Assert.Equal(text, e.Text);
                                _handleOnceCount++;
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

            await client1Mock.StartHostedServiceAsync();
            await client2Mock.StartHostedServiceAsync();

            await Task.Delay(2000);

            await serviceMock.RunAsync(async sp =>
            {
                var service = sp.GetRequiredService<IEventTestService>();
                await service.FireHandleOnce(text);
            });

            await Task.Delay(2000);

            await client1Mock.StopHostedServiceAsync();
            await client2Mock.StopHostedServiceAsync();

            Assert.Equal(1, _handleOnceCount);
        }

        [Fact]
        public async Task HandleGroupTest()
        {
            var serviceMock = Mock.Mock.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddComBoost()
                        .AddLocalService(builder =>
                        {
                            builder.AddService<EventTestService>().UseTemplate<IEventTestService>();
                            builder.AddEventHandler<DomainDistributedEventPublisher<HandleGroupEventArgs>, HandleGroupEventArgs>();
                        })
                        .AddDistributed(builder =>
                        {
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

            string text = "Hello";

            var group1client1Mock = Mock.Mock.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddComBoost()
                        .AddDistributed(builder =>
                        {
                            builder.WithGroupName("group1");
                            builder.AddDistributedEventHandler<HandleGroupEventArgs>((context, e) =>
                            {
                                Assert.Equal(text, e.Text);
                                _handleGroupCount++;
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

            var group1client2Mock = Mock.Mock.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddComBoost()
                        .AddDistributed(builder =>
                        {
                            builder.WithGroupName("group1");
                            builder.AddDistributedEventHandler<HandleGroupEventArgs>((context, e) =>
                            {
                                Assert.Equal(text, e.Text);
                                _handleGroupCount++;
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

            var group2client1Mock = Mock.Mock.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddComBoost()
                        .AddDistributed(builder =>
                        {
                            builder.WithGroupName("group2");
                            builder.AddDistributedEventHandler<HandleGroupEventArgs>((context, e) =>
                            {
                                Assert.Equal(text, e.Text);
                                _handleGroupCount++;
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

            var group2client2Mock = Mock.Mock.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddComBoost()
                        .AddDistributed(builder =>
                        {
                            builder.WithGroupName("group2");
                            builder.AddDistributedEventHandler<HandleGroupEventArgs>((context, e) =>
                            {
                                Assert.Equal(text, e.Text);
                                _handleGroupCount++;
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

            await group1client1Mock.StartHostedServiceAsync();
            await group1client2Mock.StartHostedServiceAsync();
            await group2client1Mock.StartHostedServiceAsync();
            await group2client2Mock.StartHostedServiceAsync();

            await Task.Delay(2000);

            await serviceMock.RunAsync(async sp =>
            {
                var service = sp.GetRequiredService<IEventTestService>();
                await service.FireHandleGroup(text);
            });

            await Task.Delay(2000);

            await group1client1Mock.StopHostedServiceAsync();
            await group1client2Mock.StopHostedServiceAsync();
            await group2client1Mock.StopHostedServiceAsync();
            await group2client2Mock.StopHostedServiceAsync();

            Assert.Equal(2, _handleGroupCount);
        }
    }
}
