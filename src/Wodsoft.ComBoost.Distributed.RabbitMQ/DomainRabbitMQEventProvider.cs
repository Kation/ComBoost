using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Distributed.RabbitMQ
{
    public class DomainRabbitMQEventProvider : DomainDistributedEventProvider
    {
        private IConnection _connection;
        private ConcurrentDictionary<string, IModel> _channels = new ConcurrentDictionary<string, IModel>();
        private Dictionary<string, AsyncEventingBasicConsumer> _consumers = new Dictionary<string, AsyncEventingBasicConsumer>();
        private DomainRabbitMQEventOptions _options;
        private IServiceProvider _serviceProvider;

        public DomainRabbitMQEventProvider(IDomainRabbitMQProvider provider, IOptions<DomainRabbitMQEventOptions> options, IServiceProvider serviceProvider)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));
            _connection = provider.GetConnection();
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public override Task SendEventAsync<T>(T args)
        {
            return Task.Run(() =>
            {
                var name = _options.Prefix + GetTypeName<T>();
                var channel = GetChannel<T>();
                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;
                channel.BasicPublish("", name, properties, JsonSerializer.SerializeToUtf8Bytes(args));
            });
        }

        public override void RegisterEventHandler<T>(DomainServiceEventHandler<T> handler)
        {
            var name = _options.Prefix + GetTypeName<T>();
            var channel = _connection.CreateModel();
            channel.QueueDeclare(name, true, false, false, null);
            channel.BasicQos(0, _options.PrefetchCount, false);
            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.Received += async (sender, e) =>
            {
                var args = JsonSerializer.Deserialize<T>(e.Body.Span);
                DomainContext domainContext = new EmptyDomainContext(_serviceProvider.CreateScope().ServiceProvider, default(CancellationToken));
                DomainDistributedExecutionContext executionContext = new DomainDistributedExecutionContext(domainContext);
                await handler(executionContext, args);
                channel.BasicAck(e.DeliveryTag, false);
            };
            channel.BasicConsume(name, false, consumer);
            _consumers.Add(name, consumer);
        }

        public override void UnregisterEventHandler<T>(DomainServiceEventHandler<T> handler)
        {
            var name = _options.Prefix + GetTypeName<T>();
            if (_consumers.TryGetValue(name, out var consumer))
            {
                foreach (var tag in consumer.ConsumerTags)
                    consumer.Model.BasicCancel(tag);
                consumer.Model.Close();
                consumer.Model.Dispose();
                _consumers.Remove(name);
            }
        }

        protected virtual IModel GetChannel<T>()
        {
            var name = _options.Prefix + GetTypeName<T>();
            return _channels.GetOrAdd(name, type =>
            {
                var channel = _connection.CreateModel();
                channel.QueueDeclare(name, true, false, false, null);
                return channel;
            });
        }
    }
}
