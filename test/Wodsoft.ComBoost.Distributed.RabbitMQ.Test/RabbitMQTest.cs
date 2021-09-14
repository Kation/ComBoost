using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
        volatile int _handleOnceDelayCount = 0;
        volatile int _handleMoreDelayCount = 0;

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
                            builder.UseRabbitMQ("amqp://guest:guest@127.0.0.1");
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
                            builder.UseRabbitMQ("amqp://guest:guest@127.0.0.1");
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
                            builder.UseRabbitMQ("amqp://guest:guest@127.0.0.1");
                        })
                        .AddMock();
                })
                .Build();

            await client1Mock.StartHostedServiceAsync();
            await client2Mock.StartHostedServiceAsync();

            await serviceMock.RunAsync(async sp =>
            {
                var service = sp.GetRequiredService<IEventTestService>();
                await service.FireHandleOnce(text);
            });

            await Task.Delay(1000);

            await client1Mock.StopHostedServiceAsync();
            await client2Mock.StopHostedServiceAsync();

            Assert.Equal(1, _handleOnceCount);
        }


        [Fact]
        public async Task HandleMoreTest()
        {
            var serviceMock = Mock.Mock.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddComBoost()
                        .AddLocalService(builder =>
                        {
                            builder.AddService<EventTestService>().UseTemplate<IEventTestService>();
                            builder.AddEventHandler<DomainDistributedEventPublisher<HandleMoreEventArgs>, HandleMoreEventArgs>();
                        })
                        .AddDistributed(builder =>
                        {
                            builder.UseRabbitMQ("amqp://guest:guest@127.0.0.1");
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
                            builder.AddDistributedEventHandler<HandleMoreEventArgs>((context, e) =>
                            {
                                Assert.Equal(text, e.Text);
                                _handleMoreCount++;
                                return Task.CompletedTask;
                            });
                            builder.UseRabbitMQ("amqp://guest:guest@127.0.0.1");
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
                            builder.AddDistributedEventHandler<HandleMoreEventArgs>((context, e) =>
                            {
                                Assert.Equal(text, e.Text);
                                _handleMoreCount++;
                                return Task.CompletedTask;
                            });
                            builder.UseRabbitMQ("amqp://guest:guest@127.0.0.1");
                        })
                        .AddMock();
                })
                .Build();

            await client1Mock.StartHostedServiceAsync();
            await client2Mock.StartHostedServiceAsync();

            await serviceMock.RunAsync(async sp =>
            {
                var service = sp.GetRequiredService<IEventTestService>();
                await service.FireHandleMore(text);
            });

            await Task.Delay(1000);

            await client1Mock.StopHostedServiceAsync();
            await client2Mock.StopHostedServiceAsync();

            Assert.Equal(2, _handleMoreCount);
        }

        [Fact]
        public async Task HandleOnceDelayTest()
        {
            var serviceMock = Mock.Mock.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddComBoost()
                        .AddLocalService(builder =>
                        {
                            builder.AddService<EventTestService>().UseTemplate<IEventTestService>();
                            builder.AddEventHandler<DomainDistributedEventPublisher<HandleOnceDelayEventArgs>, HandleOnceDelayEventArgs>();
                        })
                        .AddDistributed(builder =>
                        {
                            builder.UseRabbitMQ("amqp://guest:guest@127.0.0.1");
                        })
                        .AddMock();
                })
                .Build();

            string text = "Hello";

            Stopwatch stopwatch = new Stopwatch();

            var client1Mock = Mock.Mock.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddComBoost()
                        .AddDistributed(builder =>
                        {
                            builder.AddDistributedEventHandler<HandleOnceDelayEventArgs>((context, e) =>
                            {
                                stopwatch.Stop();
                                Assert.Equal(text, e.Text);
                                _handleOnceDelayCount++;
                                return Task.CompletedTask;
                            });
                            builder.UseRabbitMQ("amqp://guest:guest@127.0.0.1");
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
                            builder.AddDistributedEventHandler<HandleOnceDelayEventArgs>((context, e) =>
                            {
                                stopwatch.Stop();
                                Assert.Equal(text, e.Text);
                                _handleOnceDelayCount++;
                                return Task.CompletedTask;
                            });
                            builder.UseRabbitMQ("amqp://guest:guest@127.0.0.1");
                        })
                        .AddMock();
                })
                .Build();

            await client1Mock.StartHostedServiceAsync();
            await client2Mock.StartHostedServiceAsync();

            await serviceMock.RunAsync(async sp =>
            {
                var service = sp.GetRequiredService<IEventTestService>();
                stopwatch.Start();
                await service.FireHandleOnceDelay(text);
            });

            await Task.Delay(3000);
            Assert.True(stopwatch.ElapsedMilliseconds >= 2000);

            await client1Mock.StopHostedServiceAsync();
            await client2Mock.StopHostedServiceAsync();

            Assert.Equal(1, _handleOnceDelayCount);
        }

        [Fact]
        public async Task HandleMoreDelayTest()
        {
            var serviceMock = Mock.Mock.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddComBoost()
                        .AddLocalService(builder =>
                        {
                            builder.AddService<EventTestService>().UseTemplate<IEventTestService>();
                            builder.AddEventHandler<DomainDistributedEventPublisher<HandleMoreDelayEventArgs>, HandleMoreDelayEventArgs>();
                        })
                        .AddDistributed(builder =>
                        {
                            builder.UseRabbitMQ("amqp://guest:guest@127.0.0.1");
                        })
                        .AddMock();
                })
                .Build();

            string text = "Hello";

            Stopwatch stopwatch = new Stopwatch();

            var client1Mock = Mock.Mock.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddComBoost()
                        .AddDistributed(builder =>
                        {
                            builder.AddDistributedEventHandler<HandleMoreDelayEventArgs>((context, e) =>
                            {
                                stopwatch.Stop();
                                Assert.Equal(text, e.Text);
                                _handleMoreDelayCount++;
                                return Task.CompletedTask;
                            });
                            builder.UseRabbitMQ("amqp://guest:guest@127.0.0.1");
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
                            builder.AddDistributedEventHandler<HandleMoreDelayEventArgs>((context, e) =>
                            {
                                stopwatch.Stop();
                                Assert.Equal(text, e.Text);
                                _handleMoreDelayCount++;
                                return Task.CompletedTask;
                            });
                            builder.UseRabbitMQ("amqp://guest:guest@127.0.0.1");
                        })
                        .AddMock();
                })
                .Build();

            await client1Mock.StartHostedServiceAsync();
            await client2Mock.StartHostedServiceAsync();

            await serviceMock.RunAsync(async sp =>
            {
                var service = sp.GetRequiredService<IEventTestService>();
                stopwatch.Start();
                await service.FireHandleMoreDelay(text);
            });

            await Task.Delay(3000);
            Assert.True(stopwatch.ElapsedMilliseconds >= 2000);

            await client1Mock.StopHostedServiceAsync();
            await client2Mock.StopHostedServiceAsync();

            Assert.Equal(2, _handleMoreDelayCount);
        }
    }
}
