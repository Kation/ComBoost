﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
        private DomainServiceDistributedEventOptions _eventOptions;

        public DomainRabbitMQEventProvider(IDomainRabbitMQProvider provider, IOptions<DomainRabbitMQEventOptions> options, IOptions<DomainServiceDistributedEventOptions> eventOptions, IServiceProvider serviceProvider)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));
            _connection = provider.GetConnection();
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _eventOptions = eventOptions?.Value ?? throw new ArgumentNullException(nameof(eventOptions));
        }

        public override Task SendEventAsync<T>(T args, IReadOnlyList<string> features)
        {
            return Task.Run(() =>
            {
                var name = _options.Prefix + GetTypeName<T>();
                var channel = GetPublisherChannel<T>(features);
                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;
                if (features.Contains(DomainDistributedEventFeatures.Delay) && args is IDomainDistributedDelayEvent delayEvent)
                {
                    properties.Expiration = delayEvent.Delay.ToString();
                    name += "_DELAY";
                    channel.BasicPublish("", name, properties, JsonSerializer.SerializeToUtf8Bytes(args));
                }
                else
                {
                    if (features.Contains(DomainDistributedEventFeatures.HandleOnce) && !features.Contains(DomainDistributedEventFeatures.Group))
                        channel.BasicPublish("", name, properties, JsonSerializer.SerializeToUtf8Bytes(args));
                    else
                        channel.BasicPublish(name + "_EXCHANGE", "", properties, JsonSerializer.SerializeToUtf8Bytes(args));
                }
            });
        }

        public override void RegisterEventHandler<T>(DomainServiceEventHandler<T> handler, IReadOnlyList<string> features)
        {
            var name = _options.Prefix + GetTypeName<T>();
            var channel = _connection.CreateModel();
            string queueName;
            if (features.Contains(DomainDistributedEventFeatures.HandleOnce))
            {
                if (features.Contains(DomainDistributedEventFeatures.Group))
                {
                    channel.ExchangeDeclare(name + "_EXCHANGE", ExchangeType.Fanout);
                    queueName = name + "_" + _eventOptions.GroupName;
                    channel.QueueDeclare(queueName, true, false, false, null);
                    channel.QueueBind(queueName, name + "_EXCHANGE", "");
                }
                else
                {
                    channel.QueueDeclare(name, true, false, false, null);
                    queueName = name;
                }
            }
            else
            {
                channel.ExchangeDeclare(name + "_EXCHANGE", ExchangeType.Fanout);
                queueName = channel.QueueDeclare().QueueName;
                channel.QueueBind(queueName, name + "_EXCHANGE", "");
            }
            if (features.Contains(DomainDistributedEventFeatures.Delay))
            {
                var args = new Dictionary<string, object>();
                args["x-dead-letter-exchange"] = name + "_EXCHANGE";
                if (features.Contains(DomainDistributedEventFeatures.HandleOnce) && !features.Contains(DomainDistributedEventFeatures.Group))
                {
                    channel.ExchangeDeclare(name + "_EXCHANGE", ExchangeType.Fanout);
                    channel.QueueBind(name, name + "_EXCHANGE", "");
                }
                channel.QueueDeclare(name + "_DELAY", true, false, false, args);
            }
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
            channel.BasicConsume(queueName, false, consumer);
            _consumers.Add(name, consumer);
        }

        public override void UnregisterEventHandler<T>(DomainServiceEventHandler<T> handler, IReadOnlyList<string> features)
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

        protected virtual IModel GetPublisherChannel<T>(IReadOnlyList<string> features)
        {
            var name = _options.Prefix + GetTypeName<T>();
            return _channels.GetOrAdd(name, type =>
            {
                var channel = _connection.CreateModel();
                channel.QueueDeclare(name, true, false, false, null);
                return channel;
            });
        }

        public override bool CanHandleEvent<T>(IReadOnlyList<string> features)
        {
            var type = typeof(T);
            bool result = true;
            bool must = false;
            foreach (var feature in features)
            {
                switch (feature)
                {
                    case DomainDistributedEventFeatures.HandleOnce:
                        result = true;
                        break;
                    case DomainDistributedEventFeatures.MustHandle:
                        must = true;
                        break;
                    case DomainDistributedEventFeatures.Delay:
                        result = true;
                        break;
                    case DomainDistributedEventFeatures.Group:
                        result = true;
                        break;
                    default:
                        return false;
                }
            }
            return result && must;
        }
    }
}
