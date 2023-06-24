using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Distributed.RabbitMQ.Test.Services;
using Wodsoft.ComBoost.Mock;
using Xunit;

namespace Wodsoft.ComBoost.Distributed.RabbitMQ.Test
{
    public class RabbitMQTest
    {
        volatile int _handleOnceCount = 0;
        volatile int _handleMoreCount = 0;
        volatile int _handleGroupCount = 0;
        volatile int _handleOnceDelayCount = 0;
        volatile int _handleMoreDelayCount = 0;
        volatile int _handleOnceRetryCount = 0;

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
                            builder.UseRabbitMQ("amqp://guest:guest@127.0.0.1")
                                .AddDistributedEventPublisher<HandleOnceEventArgs>();
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
                            builder.UseRabbitMQ("amqp://guest:guest@127.0.0.1")
                                .AddDistributedEventHandler<HandleOnceEventArgs>((context, e) =>
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
                            builder.UseRabbitMQ("amqp://guest:guest@127.0.0.1")
                                .AddDistributedEventHandler<HandleOnceEventArgs>((context, e) =>
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

            await serviceMock.RunAsync(async sp =>
            {
                var service = sp.GetRequiredService<IEventTestService>();
                await service.FireHandleOnce(text);
            });

            await Task.Delay(1000);

            await client1Mock.StopAsync();
            await client2Mock.StopAsync();
            await serviceMock.StopAsync();

            Assert.Equal(1, _handleOnceCount);
        }

        [Fact]
        public async Task HandleMoreTest()
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
                            builder.UseRabbitMQ("amqp://guest:guest@127.0.0.1")
                                .AddDistributedEventPublisher<HandleMoreEventArgs>();
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
                            builder.UseRabbitMQ("amqp://guest:guest@127.0.0.1")
                                .AddDistributedEventHandler<HandleMoreEventArgs>((context, e) =>
                                {
                                    Assert.Equal(text, e.Text);
                                    _handleMoreCount++;
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
                            builder.UseRabbitMQ("amqp://guest:guest@127.0.0.1")
                                .AddDistributedEventHandler<HandleMoreEventArgs>((context, e) =>
                                {
                                    Assert.Equal(text, e.Text);
                                    _handleMoreCount++;
                                    return Task.CompletedTask;
                                });
                        })
                        .AddMock();
                })
                .Build();

            await serviceMock.StartAsync();
            await client1Mock.StartAsync();
            await client2Mock.StartAsync();

            await serviceMock.RunAsync(async sp =>
            {
                var service = sp.GetRequiredService<IEventTestService>();
                await service.FireHandleMore(text);
            });

            await Task.Delay(2000);

            await client1Mock.StopAsync();
            await client2Mock.StopAsync();
            await serviceMock.StopAsync();

            Assert.Equal(2, _handleMoreCount);
        }

        [Fact]
        public async Task HandleOnceDelayTest()
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
                            builder.UseRabbitMQ("amqp://guest:guest@127.0.0.1")
                                .AddDistributedEventPublisher<HandleOnceDelayEventArgs>();
                        })
                        .AddMock();
                })
                .Build();

            string text = "Hello";

            Stopwatch stopwatch = new Stopwatch();

            var client1Mock = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddComBoost()
                        .AddDistributed(builder =>
                        {
                            builder.UseRabbitMQ("amqp://guest:guest@127.0.0.1")
                                .AddDistributedEventHandler<HandleOnceDelayEventArgs>((context, e) =>
                                {
                                    stopwatch.Stop();
                                    Assert.Equal(text, e.Text);
                                    _handleOnceDelayCount++;
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
                            builder.UseRabbitMQ("amqp://guest:guest@127.0.0.1")
                                .AddDistributedEventHandler<HandleOnceDelayEventArgs>((context, e) =>
                                {
                                    stopwatch.Stop();
                                    Assert.Equal(text, e.Text);
                                    _handleOnceDelayCount++;
                                    return Task.CompletedTask;
                                });
                        })
                        .AddMock();
                })
                .Build();

            await serviceMock.StartAsync();
            await client1Mock.StartAsync();
            await client2Mock.StartAsync();

            await serviceMock.RunAsync(async sp =>
            {
                var service = sp.GetRequiredService<IEventTestService>();
                stopwatch.Start();
                await service.FireHandleOnceDelay(text);
            });

            await Task.Delay(8000);
            Assert.True(stopwatch.ElapsedMilliseconds >= 5000);

            await client1Mock.StopAsync();
            await client2Mock.StopAsync();
            await serviceMock.StopAsync();

            Assert.Equal(1, _handleOnceDelayCount);
        }

        [Fact]
        public async Task HandleMoreDelayTest()
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
                            builder.UseRabbitMQ("amqp://guest:guest@127.0.0.1")
                                .AddDistributedEventPublisher<HandleMoreDelayEventArgs>();
                        })
                        .AddMock();
                })
                .Build();

            string text = "Hello";

            Stopwatch stopwatch = new Stopwatch();

            var client1Mock = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddComBoost()
                        .AddDistributed(builder =>
                        {
                            builder.UseRabbitMQ("amqp://guest:guest@127.0.0.1")
                                .AddDistributedEventHandler<HandleMoreDelayEventArgs>((context, e) =>
                                {
                                    stopwatch.Stop();
                                    Assert.Equal(text, e.Text);
                                    _handleMoreDelayCount++;
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
                            builder.UseRabbitMQ("amqp://guest:guest@127.0.0.1")
                                .AddDistributedEventHandler<HandleMoreDelayEventArgs>((context, e) =>
                                {
                                    stopwatch.Stop();
                                    Assert.Equal(text, e.Text);
                                    _handleMoreDelayCount++;
                                    return Task.CompletedTask;
                                });
                        })
                        .AddMock();
                })
                .Build();

            await serviceMock.StartAsync();
            await client1Mock.StartAsync();
            await client2Mock.StartAsync();

            await serviceMock.RunAsync(async sp =>
            {
                var service = sp.GetRequiredService<IEventTestService>();
                stopwatch.Start();
                await service.FireHandleMoreDelay(text);
            });

            await Task.Delay(8000);
            Assert.True(stopwatch.ElapsedMilliseconds >= 5000);

            await client1Mock.StopAsync();
            await client2Mock.StopAsync();
            await serviceMock.StopAsync();

            Assert.Equal(2, _handleMoreDelayCount);
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
                            builder.UseRabbitMQ("amqp://guest:guest@127.0.0.1")
                                .AddDistributedEventPublisher<HandleGroupEventArgs>();
                        })
                        .AddMock();
                })
                .Build();

            string text = "Hello";

            Stopwatch stopwatch = new Stopwatch();

            var group1Client1Mock = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddComBoost()
                        .AddDistributed(builder =>
                        {
                            builder.UseRabbitMQ("amqp://guest:guest@127.0.0.1")
                                .AddDistributedEventHandler<HandleGroupEventArgs>((context, e) =>
                                {
                                    stopwatch.Stop();
                                    Assert.Equal(text, e.Text);
                                    _handleGroupCount++;
                                    return Task.CompletedTask;
                                })
                                .WithGroupName("group1");
                        })
                        .AddMock();
                })
                .Build();

            var group1Client2Mock = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddComBoost()
                        .AddDistributed(builder =>
                        {
                            builder.UseRabbitMQ("amqp://guest:guest@127.0.0.1")
                                .AddDistributedEventHandler<HandleGroupEventArgs>((context, e) =>
                                {
                                    stopwatch.Stop();
                                    Assert.Equal(text, e.Text);
                                    _handleGroupCount++;
                                    return Task.CompletedTask;
                                })
                                .WithGroupName("group1");
                        })
                        .AddMock();
                })
                .Build();

            var group2Client1Mock = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddComBoost()
                        .AddDistributed(builder =>
                        {
                            builder.UseRabbitMQ("amqp://guest:guest@127.0.0.1")
                                .AddDistributedEventHandler<HandleGroupEventArgs>((context, e) =>
                                {
                                    stopwatch.Stop();
                                    Assert.Equal(text, e.Text);
                                    _handleGroupCount++;
                                    return Task.CompletedTask;
                                })
                                .WithGroupName("group2");
                        })
                        .AddMock();
                })
                .Build();

            var group2Client2Mock = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddComBoost()
                        .AddDistributed(builder =>
                        {
                            builder.UseRabbitMQ("amqp://guest:guest@127.0.0.1")
                                .AddDistributedEventHandler<HandleGroupEventArgs>((context, e) =>
                                {
                                    stopwatch.Stop();
                                    Assert.Equal(text, e.Text);
                                    _handleGroupCount++;
                                    return Task.CompletedTask;
                                })
                                .WithGroupName("group2");
                        })
                        .AddMock();
                })
                .Build();

            await serviceMock.StartAsync();
            await group1Client1Mock.StartAsync();
            await group1Client2Mock.StartAsync();
            await group2Client1Mock.StartAsync();
            await group2Client2Mock.StartAsync();

            await serviceMock.RunAsync(async sp =>
            {
                var service = sp.GetRequiredService<IEventTestService>();
                stopwatch.Start();
                await service.FireHandleGroup(text);
            });

            await Task.Delay(3000);

            await group1Client1Mock.StopAsync();
            await group1Client2Mock.StopAsync();
            await group2Client1Mock.StopAsync();
            await group2Client2Mock.StopAsync();
            await serviceMock.StopAsync();

            Assert.Equal(2, _handleGroupCount);
        }

        [Fact]
        public async Task HandleGroupDelayTest()
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
                            builder.UseRabbitMQ("amqp://guest:guest@127.0.0.1")
                                .AddDistributedEventPublisher<HandleGroupDelayEventArgs>();
                        })
                        .AddMock();
                })
                .Build();

            string text = "Hello";

            Stopwatch stopwatch = new Stopwatch();

            var group1Client1Mock = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddComBoost()
                        .AddDistributed(builder =>
                        {
                            builder.UseRabbitMQ("amqp://guest:guest@127.0.0.1")
                                .AddDistributedEventHandler<HandleGroupDelayEventArgs>((context, e) =>
                                {
                                    stopwatch.Stop();
                                    Assert.Equal(text, e.Text);
                                    _handleGroupCount++;
                                    return Task.CompletedTask;
                                })
                                .WithGroupName("group1");
                        })
                        .AddMock();
                })
                .Build();

            var group1Client2Mock = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddComBoost()
                        .AddDistributed(builder =>
                        {
                            builder.UseRabbitMQ("amqp://guest:guest@127.0.0.1")
                                .AddDistributedEventHandler<HandleGroupDelayEventArgs>((context, e) =>
                                {
                                    stopwatch.Stop();
                                    Assert.Equal(text, e.Text);
                                    _handleGroupCount++;
                                    return Task.CompletedTask;
                                })
                                .WithGroupName("group1");
                        })
                        .AddMock();
                })
                .Build();

            var group2Client1Mock = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddComBoost()
                        .AddDistributed(builder =>
                        {
                            builder.UseRabbitMQ("amqp://guest:guest@127.0.0.1")
                                .AddDistributedEventHandler<HandleGroupDelayEventArgs>((context, e) =>
                                {
                                    stopwatch.Stop();
                                    Assert.Equal(text, e.Text);
                                    _handleGroupCount++;
                                    return Task.CompletedTask;
                                })
                                .WithGroupName("group2");
                        })
                        .AddMock();
                })
                .Build();

            var group2Client2Mock = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddComBoost()
                        .AddDistributed(builder =>
                        {
                            builder.UseRabbitMQ("amqp://guest:guest@127.0.0.1")
                                .AddDistributedEventHandler<HandleGroupDelayEventArgs>((context, e) =>
                                {
                                    stopwatch.Stop();
                                    Assert.Equal(text, e.Text);
                                    _handleGroupCount++;
                                    return Task.CompletedTask;
                                })
                                .WithGroupName("group2");
                        })
                        .AddMock();
                })
                .Build();

            await serviceMock.StartAsync();
            await group1Client1Mock.StartAsync();
            await group1Client2Mock.StartAsync();
            await group2Client1Mock.StartAsync();
            await group2Client2Mock.StartAsync();

            await serviceMock.RunAsync(async sp =>
            {
                var service = sp.GetRequiredService<IEventTestService>();
                stopwatch.Start();
                await service.FireHandleGroupDelay(text);
            });

            await Task.Delay(8000);
            Assert.True(stopwatch.ElapsedMilliseconds >= 5000);

            await group1Client1Mock.StopAsync();
            await group1Client2Mock.StopAsync();
            await group2Client1Mock.StopAsync();
            await group2Client2Mock.StopAsync();
            await serviceMock.StopAsync();

            Assert.Equal(2, _handleGroupCount);
        }

        [Fact]
        public async Task HandleOnceRetryTest()
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
                            builder.UseRabbitMQ("amqp://guest:guest@127.0.0.1")
                                .AddDistributedEventPublisher<HandleOnceRetryEventArgs>();
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
                            builder.UseRabbitMQ("amqp://guest:guest@127.0.0.1")
                                .AddDistributedEventHandler<HandleOnceRetryEventArgs>((context, e) =>
                                {
                                    Assert.Equal(text, e.Text);
                                    _handleOnceRetryCount++;
                                    if (e.RetryCount < 3)
                                        throw new Exception();
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
                            builder.UseRabbitMQ("amqp://guest:guest@127.0.0.1")
                                .AddDistributedEventHandler<HandleOnceRetryEventArgs>((context, e) =>
                                {
                                    Assert.Equal(text, e.Text);
                                    _handleOnceRetryCount++;
                                    if (e.RetryCount < 3)
                                        throw new Exception();
                                    return Task.CompletedTask;
                                });
                        })
                        .AddMock();
                })
                .Build();

            await serviceMock.StartAsync();
            await client1Mock.StartAsync();
            await client2Mock.StartAsync();

            await serviceMock.RunAsync(async sp =>
            {
                var service = sp.GetRequiredService<IEventTestService>();
                await service.FireHandleOnceRetry(text);
            });

            await Task.Delay(1000);
            Assert.Equal(1, _handleOnceRetryCount);
            await Task.Delay(5000);
            Assert.Equal(2, _handleOnceRetryCount);
            await Task.Delay(10000);
            Assert.Equal(3, _handleOnceRetryCount);
            await Task.Delay(15000);
            Assert.Equal(4, _handleOnceRetryCount);

            await client1Mock.StopAsync();
            await client2Mock.StopAsync();
            await serviceMock.StopAsync();
        }
    }
}
