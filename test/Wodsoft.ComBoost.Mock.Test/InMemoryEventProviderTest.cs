using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Mock;
using Wodsoft.ComBoost.Test.EventServices;
using Xunit;

namespace Wodsoft.ComBoost.Mock.Test
{
    public class InMemoryEventProviderTest
    {
        volatile int _handleOnceCount = 0;
        volatile int _handleMoreCount = 0;
        volatile int _handleGroupCount = 0;
        volatile int _handleOnceDelayCount = 0;
        volatile int _handleMoreDelayCount = 0;
        volatile int _handleRetryCount = 0;
        volatile int _handleRetryPluginCount = 0;
        volatile int _handleSingleCount = 0;
        volatile int _handleGroupSingleCount = 0;
        volatile int _handleMoreDelayPluginCount = 0;
        volatile int _handleGroupRetryCount = 0;

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
                            builder.UseInMemory(options => options.IsAsyncEvent = true)
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
                            builder.UseInMemory(options => options.IsAsyncEvent = true)
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
                            builder.UseInMemory(options => options.IsAsyncEvent = true)
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
                            builder.UseInMemory(options => options.IsAsyncEvent = true)
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
                            builder.UseInMemory(options => options.IsAsyncEvent = true)
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
                            builder.UseInMemory(options => options.IsAsyncEvent = true)
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
                            builder.UseInMemory(options => options.IsAsyncEvent = true)
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
                            builder.UseInMemory(options => options.IsAsyncEvent = true)
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
                            builder.UseInMemory(options => options.IsAsyncEvent = true)
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
                            builder.UseInMemory(options => options.IsAsyncEvent = true)
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
                            builder.UseInMemory(options => options.IsAsyncEvent = true)
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
                            builder.UseInMemory(options => options.IsAsyncEvent = true)
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
                            builder.UseInMemory(options => options.IsAsyncEvent = true)
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
                            builder.UseInMemory(options => options.IsAsyncEvent = true)
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
                            builder.UseInMemory(options => options.IsAsyncEvent = true)
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
                            builder.UseInMemory(options => options.IsAsyncEvent = true)
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
                            builder.UseInMemory(options => options.IsAsyncEvent = true)
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
                            builder.UseInMemory(options => options.IsAsyncEvent = true)
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
                            builder.UseInMemory(options => options.IsAsyncEvent = true)
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
        public async Task HandleSingleTest()
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
                            builder.UseInMemory(options => options.IsAsyncEvent = true)
                                .AddDistributedEventPublisher<HandleSingleEventArgs>();
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
                            builder.UseInMemory(options => options.IsAsyncEvent = true)
                                .AddDistributedEventHandler<HandleSingleEventArgs>(async (context, e) =>
                                {
                                    Assert.Equal(text, e.Text);
                                    await Task.Delay(2000);
                                    _handleSingleCount++;
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
                            builder.UseInMemory(options => options.IsAsyncEvent = true)
                                .AddDistributedEventHandler<HandleSingleEventArgs>(async (context, e) =>
                                {
                                    Assert.Equal(text, e.Text);
                                    await Task.Delay(2000);
                                    _handleSingleCount++;
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
                await service.FireHandleSingle(text);
                await service.FireHandleSingle(text);
            });

            await Task.Delay(3000);
            Assert.Equal(1, _handleSingleCount);

            await Task.Delay(2000);
            Assert.Equal(2, _handleSingleCount);

            await client1Mock.StopAsync();
            await client2Mock.StopAsync();
            await serviceMock.StopAsync();

        }

        [Fact]
        public async Task HandleGroupSingleTest()
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
                            builder.UseInMemory(options => options.IsAsyncEvent = true)
                                .AddDistributedEventPublisher<HandleGroupSingleEventArgs>();
                        })
                        .AddMock();
                })
                .Build();

            string text = "Hello";

            var group1Client1Mock = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddComBoost()
                        .AddDistributed(builder =>
                        {
                            builder.UseInMemory(options => options.IsAsyncEvent = true)
                                .AddDistributedEventHandler<HandleGroupSingleEventArgs>(async (context, e) =>
                                {
                                    Assert.Equal(text, e.Text);
                                    await Task.Delay(2000);
                                    _handleGroupSingleCount++;
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
                            builder.UseInMemory(options => options.IsAsyncEvent = true)
                                .AddDistributedEventHandler<HandleGroupSingleEventArgs>(async (context, e) =>
                                {
                                    Assert.Equal(text, e.Text);
                                    await Task.Delay(2000);
                                    _handleGroupSingleCount++;
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
                            builder.UseInMemory(options => options.IsAsyncEvent = true)
                                .AddDistributedEventHandler<HandleGroupSingleEventArgs>(async (context, e) =>
                                {
                                    Assert.Equal(text, e.Text);
                                    await Task.Delay(2000);
                                    _handleGroupSingleCount++;
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
                            builder.UseInMemory(options => options.IsAsyncEvent = true)
                                .AddDistributedEventHandler<HandleGroupSingleEventArgs>(async (context, e) =>
                                {
                                    Assert.Equal(text, e.Text);
                                    await Task.Delay(2000);
                                    _handleGroupSingleCount++;
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
                await service.FireHandleGroupSingle(text);
                await service.FireHandleGroupSingle(text);
            });

            await Task.Delay(3000);
            Assert.Equal(2, _handleGroupSingleCount);

            await Task.Delay(2000);
            Assert.Equal(4, _handleGroupSingleCount);

            await group1Client1Mock.StopAsync();
            await group1Client2Mock.StopAsync();
            await group2Client1Mock.StopAsync();
            await group2Client2Mock.StopAsync();
            await serviceMock.StopAsync();
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
                            builder.UseInMemory(options => options.IsAsyncEvent = true).AddDistributedEventPublisher<HandleMoreDelayPluginEventArgs>();
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
                            builder.UseInMemory(options => options.IsAsyncEvent = true).AddDistributedEventHandler<HandleMoreDelayPluginEventArgs>((context, e) =>
                            {
                                Assert.Equal(text, e.Text);
                                _handleMoreDelayPluginCount++;
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
                            builder.UseInMemory(options => options.IsAsyncEvent = true).AddDistributedEventHandler<HandleMoreDelayPluginEventArgs>((context, e) =>
                            {
                                Assert.Equal(text, e.Text);
                                _handleMoreDelayPluginCount++;
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
                await service.FireHandleMoreDelayPlugin(text, 5000);
                await service.FireHandleMoreDelayPlugin(text, 2000);
            });

            await Task.Delay(3000);
            Assert.Equal(2, _handleMoreDelayPluginCount);
            await Task.Delay(3000);
            Assert.Equal(4, _handleMoreDelayPluginCount);

            await client1Mock.StopAsync();
            await client2Mock.StopAsync();
            await serviceMock.StopAsync();
        }

        [Fact]
        public async Task HandleRetryTest()
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
                            builder.UseInMemory(options => options.IsAsyncEvent = true).AddDistributedEventPublisher<HandleRetryPluginEventArgs>();
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
                            builder.UseInMemory(options => options.IsAsyncEvent = true).AddDistributedEventHandler<HandleRetryPluginEventArgs>((context, e) =>
                            {
                                Assert.Equal(text, e.Text);
                                _handleRetryPluginCount++;
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
                            builder.UseInMemory(options => options.IsAsyncEvent = true).AddDistributedEventHandler<HandleRetryPluginEventArgs>((context, e) =>
                            {
                                Assert.Equal(text, e.Text);
                                _handleRetryPluginCount++;
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
                await service.FireHandleRetryPlugin(text);
            });

            await Task.Delay(1000);
            Assert.Equal(1, _handleRetryPluginCount);
            await Task.Delay(5000);
            Assert.Equal(2, _handleRetryPluginCount);
            await Task.Delay(10000);
            Assert.Equal(3, _handleRetryPluginCount);
            await Task.Delay(15000);
            Assert.Equal(4, _handleRetryPluginCount);

            await client1Mock.StopAsync();
            await client2Mock.StopAsync();
            await serviceMock.StopAsync();
        }

    }
}
