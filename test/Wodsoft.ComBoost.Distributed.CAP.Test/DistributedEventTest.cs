﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
            var serviceMock = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddComBoost()
                        .AddLocalService(builder =>
                        {
                            builder.AddService<EventTestService>().UseTemplate<IEventTestService>();
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
                            }).AddDistributedEventPublisher<HandleOnceEventArgs>();
                        })
                        .AddMock();                    
                })
                .Build();

            string text = "Hello";

            var client1Mock = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddComBoost()
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
                            }).AddDistributedEventHandler<HandleOnceEventArgs>((context, e) =>
                            {
                                Assert.Equal(text, e.Text);
                                _handleOnceCount++;
                                return Task.CompletedTask;
                            });
                        })
                        .AddMock();
                })
                .Build();

            var client2Mock = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddComBoost()
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
                            }).AddDistributedEventHandler<HandleOnceEventArgs>((context, e) =>
                            {
                                Assert.Equal(text, e.Text);
                                _handleOnceCount++;
                                return Task.CompletedTask;
                            });
                        })
                        .AddMock();
                })
                .Build();

            await serviceMock.StartAsync();
            await client1Mock.StartAsync();
            await client2Mock.StartAsync();

            await Task.Delay(2000);

            await serviceMock.RunAsync(async sp =>
            {
                var service = sp.GetRequiredService<IEventTestService>();
                await service.FireHandleOnce(text);
            });

            await Task.Delay(2000);

            await serviceMock.StopAsync();
            await client1Mock.StopAsync();
            await client2Mock.StopAsync();

            Assert.Equal(1, _handleOnceCount);
        }

        [Fact]
        public async Task HandleGroupTest()
        {
            var serviceMock = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddComBoost()
                        .AddLocalService(builder =>
                        {
                            builder.AddService<EventTestService>().UseTemplate<IEventTestService>();
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
                            }).AddDistributedEventPublisher<HandleGroupEventArgs>();
                        })
                        .AddMock();
                })
                .Build();

            string text = "Hello";

            var group1client1Mock = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddComBoost()
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
                            }).AddDistributedEventHandler<HandleGroupEventArgs>((context, e) =>
                            {
                                Assert.Equal(text, e.Text);
                                _handleGroupCount++;
                                return Task.CompletedTask;
                            }).WithGroupName("group1");
                        })
                        .AddMock();
                })
                .Build();

            var group1client2Mock = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddComBoost()
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
                            }).AddDistributedEventHandler<HandleGroupEventArgs>((context, e) =>
                            {
                                Assert.Equal(text, e.Text);
                                _handleGroupCount++;
                                return Task.CompletedTask;
                            }).WithGroupName("group1");
                        })
                        .AddMock();
                })
                .Build();

            var group2client1Mock = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddComBoost()
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
                            }).AddDistributedEventHandler<HandleGroupEventArgs>((context, e) =>
                            {
                                Assert.Equal(text, e.Text);
                                _handleGroupCount++;
                                return Task.CompletedTask;
                            }).WithGroupName("group2");
                        })
                        .AddMock();
                })
                .Build();

            var group2client2Mock = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddComBoost()
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
                            }).AddDistributedEventHandler<HandleGroupEventArgs>((context, e) =>
                            {
                                Assert.Equal(text, e.Text);
                                _handleGroupCount++;
                                return Task.CompletedTask;
                            }).WithGroupName("group2");
                        })
                        .AddMock();
                })
                .Build();

            await serviceMock.StartAsync();
            await group1client1Mock.StartAsync();
            await group1client2Mock.StartAsync();
            await group2client1Mock.StartAsync();
            await group2client2Mock.StartAsync();

            await Task.Delay(2000);

            await serviceMock.RunAsync(async sp =>
            {
                var service = sp.GetRequiredService<IEventTestService>();
                await service.FireHandleGroup(text);
            });

            await Task.Delay(2000);

            await serviceMock.StopAsync();
            await group1client1Mock.StopAsync();
            await group1client2Mock.StopAsync();
            await group2client1Mock.StopAsync();
            await group2client2Mock.StopAsync();

            Assert.Equal(2, _handleGroupCount);
        }
    }
}
