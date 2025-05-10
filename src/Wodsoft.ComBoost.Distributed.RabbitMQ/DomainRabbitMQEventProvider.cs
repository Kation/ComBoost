using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Distributed.RabbitMQ
{
    public class DomainRabbitMQEventProvider : DomainDistributedEventProvider
    {
        private IConnection? _connection;
        private ConcurrentDictionary<string, IModel> _channels = new ConcurrentDictionary<string, IModel>();
        private Dictionary<string, AsyncEventingBasicConsumer> _consumers = new Dictionary<string, AsyncEventingBasicConsumer>();
        private DomainRabbitMQOptions _options;
        private IServiceProvider _serviceProvider;
        private DomainServiceDistributedEventOptions<DomainRabbitMQEventProvider> _eventOptions;
        private ILogger _logger;

        public DomainRabbitMQEventProvider(DomainRabbitMQOptions options, DomainServiceDistributedEventOptions<DomainRabbitMQEventProvider> eventOptions, IServiceProvider serviceProvider,
            ILogger<DomainRabbitMQEventProvider> logger)
        {
            _logger = logger;
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _eventOptions = eventOptions ?? throw new ArgumentNullException(nameof(eventOptions));
        }

        private void _connection_CallbackException(object sender, CallbackExceptionEventArgs e)
        {
            _logger.LogError(e.Exception, "RabbitMQ connection callback throw exception.");
        }

        private void _connection_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            _logger.LogError("RabbitMQ connection shutdown.");
        }

        private void _connection_ConnectionUnblocked(object sender, EventArgs e)
        {
            _logger.LogInformation("RabbitMQ connection unblocked.");
        }

        private void _connection_ConnectionBlocked(object sender, ConnectionBlockedEventArgs e)
        {
            _logger.LogWarning("RabbitMQ connection blocked. " + e.Reason);
        }

#if NETSTANDARD2_0
        public override async Task SendEventAsync<T>(T args, IReadOnlyList<string> features)
#else
        public override async ValueTask SendEventAsync<T>(T args, IReadOnlyList<string> features)
#endif
        {
            if (_connection == null)
                throw new InvalidOperationException("未开启RabbitMQ服务。");
            await Task.Run(() =>
            {
                var name = _options.Prefix + GetTypeName<T>();
                var channel = GetPublisherChannel<T>(features);
                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;
                if (features.Contains(DomainDistributedEventFeatures.Delay) && args is IDomainDistributedDelayEvent delayEvent)
                {
                    if (_options.UseDelayedMessagePlugin)
                    {
                        Dictionary<string, object> headers = new Dictionary<string, object>();
                        headers["x-delay"] = delayEvent.Delay;
                        properties.Headers = headers;
                        name += "_EXCHANGE";
                        channel.BasicPublish(name, "", properties, JsonSerializer.SerializeToUtf8Bytes(args));
                    }
                    else
                    {
                        properties.Expiration = delayEvent.Delay.ToString();
                        name += "_DELAY";
                        channel.BasicPublish("", name, properties, JsonSerializer.SerializeToUtf8Bytes(args));
                    }
                }
                else if (features.Contains(DomainDistributedEventFeatures.Retry) && args is IDomainDistributedRetryEvent retryEvent && retryEvent.RetryCount != 0)
                {
                    var retryTimesAttribute = typeof(T).GetCustomAttribute<DomainDistributedEventRetryTimesAttribute>();
                    if (retryTimesAttribute == null)
                        throw new InvalidOperationException("A distributed event can retry means that must have \"DomainDistributedEventRetryTimesAttribute\" attribute.");
                    if (retryEvent.RetryCount > retryTimesAttribute.Times.Length)
                    {
                        _logger.LogWarning($"RabbitMQ event retry count more than limit. Event: \"{typeof(T).FullName}\".");
                        return;
                    }
                    if (_options.UseDelayedMessagePlugin)
                    {
                        Dictionary<string, object> headers = new Dictionary<string, object>();
                        headers["x-delay"] = retryTimesAttribute.Times[retryEvent.RetryCount - 1];
                        properties.Headers = headers;
                        name += "_EXCHANGE";
                        channel.BasicPublish(name, "", properties, JsonSerializer.SerializeToUtf8Bytes(args));
                    }
                    else
                    {
                        name += "_RETRY_" + retryTimesAttribute.Times[retryEvent.RetryCount - 1];
                        channel.BasicPublish("", name, properties, JsonSerializer.SerializeToUtf8Bytes(args));
                    }
                }
                else
                {
                    if (features.Contains(DomainDistributedEventFeatures.HandleOnce) && !features.Contains(DomainDistributedEventFeatures.Group))
                        channel.BasicPublish("", name, properties, JsonSerializer.SerializeToUtf8Bytes(args));
                    else
                        channel.BasicPublish(name + "_EXCHANGE", "", properties, JsonSerializer.SerializeToUtf8Bytes(args));
                }
                _logger.LogInformation($"RabbitMQ event publish successfully. Event: \"{typeof(T).FullName}\".");
            });
        }

#if NETSTANDARD2_0
        public override Task RegisterEventHandlerAsync<T>(DomainServiceEventHandler<T> handler, IReadOnlyList<string> features)
#else
        public override ValueTask RegisterEventHandlerAsync<T>(DomainServiceEventHandler<T> handler, IReadOnlyList<string> features)
#endif
        {
            if (_connection == null)
                throw new InvalidOperationException("未开启RabbitMQ服务。");
            var name = _options.Prefix + GetTypeName<T>();
            var channel = _connection.CreateModel();
            string queueName;
            if (features.Contains(DomainDistributedEventFeatures.HandleOnce))
            {
                var args = new Dictionary<string, object>();
                if (_options.UseQuorum)
                    args["x-queue-type"] = "quorum";
                if (features.Contains(DomainDistributedEventFeatures.SingleHandler))
                    args["x-single-active-consumer"] = true;
                if (features.Contains(DomainDistributedEventFeatures.Group))
                {
                    if (features.Contains(DomainDistributedEventFeatures.Delay) && _options.UseDelayedMessagePlugin)
                    {
                        var exArgs = new Dictionary<string, object>();
                        exArgs["x-delayed-type"] = "direct";
                        channel.ExchangeDeclare(name + "_EXCHANGE", "x-delayed-message", true, false, exArgs);
                    }
                    else
                    {
                        channel.ExchangeDeclare(name + "_EXCHANGE", ExchangeType.Fanout);
                    }
                    queueName = name + "_" + _eventOptions.GroupName;
                    channel.QueueDeclare(queueName, true, false, false, args);
                    channel.QueueBind(queueName, name + "_EXCHANGE", "");
                }
                else
                {
                    channel.QueueDeclare(name, true, false, false, args);
                    queueName = name;
                }
            }
            else
            {
                if (features.Contains(DomainDistributedEventFeatures.Delay) && _options.UseDelayedMessagePlugin)
                {
                    var args = new Dictionary<string, object>();
                    args["x-delayed-type"] = "direct";
                    channel.ExchangeDeclare(name + "_EXCHANGE", "x-delayed-message", true, false, args);
                }
                else
                {
                    channel.ExchangeDeclare(name + "_EXCHANGE", ExchangeType.Fanout);
                }
                queueName = channel.QueueDeclare().QueueName;
                channel.QueueBind(queueName, name + "_EXCHANGE", "");
            }
            if (features.Contains(DomainDistributedEventFeatures.Delay))
            {
                if (features.Contains(DomainDistributedEventFeatures.HandleOnce) && !features.Contains(DomainDistributedEventFeatures.Group))
                {
                    if (_options.UseDelayedMessagePlugin)
                    {
                        var args = new Dictionary<string, object>();
                        args["x-delayed-type"] = "direct";
                        channel.ExchangeDeclare(name + "_EXCHANGE", "x-delayed-message", true, false, args);
                    }
                    else
                    {
                        channel.ExchangeDeclare(name + "_EXCHANGE", ExchangeType.Fanout);
                    }
                    channel.QueueBind(name, name + "_EXCHANGE", "");
                }
                if (!_options.UseDelayedMessagePlugin)
                {
                    var args = new Dictionary<string, object>();
                    if (_options.UseQuorum && _options.UseTtlQuorum)
                        args["x-queue-type"] = "quorum";
                    args["x-dead-letter-exchange"] = name + "_EXCHANGE";
                    channel.QueueDeclare(name + "_DELAY", true, false, false, args);
                }
            }
            else if (features.Contains(DomainDistributedEventFeatures.Retry))
            {
                var retryTimesAttribute = typeof(T).GetCustomAttribute<DomainDistributedEventRetryTimesAttribute>();
                if (retryTimesAttribute == null)
                    throw new InvalidOperationException("A distributed event can retry means that must have \"DomainDistributedEventRetryTimesAttribute\" attribute.");

                if (_options.UseDelayedMessagePlugin)
                {
                    var args = new Dictionary<string, object>();
                    args["x-delayed-type"] = "direct";
                    channel.ExchangeDeclare(name + "_EXCHANGE", "x-delayed-message", true, false, args);
                }
                else
                {
                    channel.ExchangeDeclare(name + "_EXCHANGE", ExchangeType.Fanout);
                }
                channel.QueueBind(name, name + "_EXCHANGE", "");
                if (!_options.UseDelayedMessagePlugin)
                {
                    for (int i = 0; i < retryTimesAttribute.Times.Length; i++)
                    {
                        var time = retryTimesAttribute.Times[i];
                        if (i > 0 && time == retryTimesAttribute.Times[i - 1])
                            continue;
                        var args = new Dictionary<string, object>();
                        if (_options.UseQuorum && _options.UseTtlQuorum)
                            args["x-queue-type"] = "quorum";
                        args["x-message-ttl"] = time;
                        if (features.Contains(DomainDistributedEventFeatures.Group))
                        {
                            args["x-dead-letter-exchange"] = name + "_EXCHANGE";
                        }
                        else
                        {
                            args["x-dead-letter-exchange"] = name + "_EXCHANGE";
                        }
                        channel.QueueDeclare(name + "_RETRY_" + time, true, false, false, args);
                    }
                }
            }
            var concurrentAttribute = typeof(T).GetCustomAttribute<DomainDistributedEventConcurrentAttribute>();
            if (concurrentAttribute == null)
                channel.BasicQos(0, _options.PrefetchCount, false);
            else
                channel.BasicQos(0, (ushort)concurrentAttribute.Count, false);
            var consumer = new AsyncEventingBasicConsumer(channel);
            var logger = _serviceProvider.GetRequiredService<ILogger<DomainServiceEventHandler<T>>>();
            consumer.ConsumerCancelled += (sender, e) =>
            {
                logger.LogError($"RabbitMQ consumer of \"{typeof(T).FullName}\" cancelled.");
                return Task.CompletedTask;
            };
            consumer.Shutdown += (sender, e) =>
            {
                logger.LogError($"RabbitMQ consumer of \"{typeof(T).FullName}\" shutdown. " + e.ReplyText);
                return Task.CompletedTask;
            };
            consumer.Unregistered += (sender, e) =>
            {
                logger.LogInformation($"RabbitMQ consumer of \"{typeof(T).FullName}\" unregistered.");
                return Task.CompletedTask;
            };
            consumer.Registered += (sender, e) =>
            {
                logger.LogInformation($"RabbitMQ consumer of \"{typeof(T).FullName}\" registered.");
                return Task.CompletedTask;
            };
            consumer.Received += async (sender, e) =>
            {
                bool isAcked = false;
                try
                {
                    using (logger.BeginScope(new DomainRabbitMQLogState(typeof(T).FullName, e.Exchange, e.Redelivered, e.DeliveryTag)))
                    {
                        logger.LogInformation("RabbitMQ starting handle event.");
                        var args = JsonSerializer.Deserialize<T>(e.Body.Span)!;
                        using (var scope = _serviceProvider.CreateScope())
                        {
                            DomainContext domainContext = new EmptyDomainContext(scope.ServiceProvider, default(CancellationToken));
                            DomainDistributedExecutionContext executionContext = new DomainDistributedExecutionContext(domainContext);
                            bool isSuccess = false;
                            try
                            {
                                await handler(executionContext, args);
                                isSuccess = true;
                            }
                            catch (Exception ex)
                            {
                                if (args is IDomainDistributedRetryEvent retryEvent)
                                {
                                    retryEvent.RetryCount++;
                                    logger.LogError(ex, "RabbitMQ event handle error and going to retry it.");
                                    bool retrySuccess = false;
                                    try
                                    {
                                        await SendEventAsync(args, features);
                                        retrySuccess = true;
                                    }
                                    catch
                                    {
                                        channel.BasicNack(e.DeliveryTag, false, true);
                                        isAcked = true;
                                    }
                                    if (retrySuccess)
                                    {
                                        channel.BasicAck(e.DeliveryTag, false);
                                        isAcked = true;
                                    }
                                }
                                else
                                {
                                    channel.BasicNack(e.DeliveryTag, false, true);
                                    isAcked = true;
                                }
                                logger.LogError(ex, "RabbitMQ event handle error.");
                            }
                            if (isSuccess)
                            {
                                channel.BasicAck(e.DeliveryTag, false);
                                isAcked = true;
                                logger.LogInformation("RabbitMQ event handle successfully.");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "RabbitMQ operation error.");
                    if (!isAcked)
                        channel.BasicNack(e.DeliveryTag, false, true);
                }
            };
            channel.BasicConsume(queueName, false, consumer);
            _consumers.Add(name, consumer);
#if NETSTANDARD2_0
            return Task.CompletedTask;
#else
            return default;
#endif
        }

#if NETSTANDARD2_0
        public override Task UnregisterEventHandlerAsync<T>(DomainServiceEventHandler<T> handler, IReadOnlyList<string> features)
#else
        public override ValueTask UnregisterEventHandlerAsync<T>(DomainServiceEventHandler<T> handler, IReadOnlyList<string> features)
#endif
        {
            if (_connection == null)
                throw new InvalidOperationException("未开启RabbitMQ服务。");
            var name = _options.Prefix + GetTypeName<T>();
            if (_consumers.TryGetValue(name, out var consumer))
            {
                foreach (var tag in consumer.ConsumerTags)
                    consumer.Model.BasicCancel(tag);
                consumer.Model.Close();
                consumer.Model.Dispose();
                _consumers.Remove(name);
            }
#if NETSTANDARD2_0
            return Task.CompletedTask;
#else
            return default;
#endif
        }

        protected virtual IModel GetPublisherChannel<T>(IReadOnlyList<string> features)
        {
            var name = _options.Prefix + GetTypeName<T>();
            return _channels.GetOrAdd(name, type =>
            {
                var channel = _connection!.CreateModel();
                if (features.Contains(DomainDistributedEventFeatures.HandleOnce))
                {
                    var args = new Dictionary<string, object>();
                    if (_options.UseQuorum)
                        args["x-queue-type"] = "quorum";
                    if (features.Contains(DomainDistributedEventFeatures.SingleHandler))
                        args["x-single-active-consumer"] = true;
                    channel.QueueDeclare(name, true, false, false, args);
                }
                return channel;
            });
        }

        public override bool CanHandleEvent<T>(IReadOnlyList<string> features)
        {
            var type = typeof(T);
            bool result = true;
            bool must = false;
            bool retry = false;
            bool once = false;
            bool group = false;
            bool delay = false;
            foreach (var feature in features)
            {
                switch (feature)
                {
                    case DomainDistributedEventFeatures.HandleOnce:
                        result = true;
                        once = true;
                        break;
                    case DomainDistributedEventFeatures.MustHandle:
                        must = true;
                        break;
                    case DomainDistributedEventFeatures.Delay:
                        result = true;
                        delay = true;
                        break;
                    case DomainDistributedEventFeatures.Group:
                        group = true;
                        result = true;
                        break;
                    case DomainDistributedEventFeatures.Retry:
                        result = true;
                        retry = true;
                        break;
                    case DomainDistributedEventFeatures.SingleHandler:
                        result = true;
                        break;
                    default:
                        return false;
                }
            }
            if (retry && !once)
                return false;
            if (retry && group)
                return false;
            if (retry && delay)
                return false;
            return result && must;
        }

        public override Task StartAsync()
        {
            if (_connection != null)
                return Task.CompletedTask;
            ConnectionFactory factory = new ConnectionFactory();
            factory.DispatchConsumersAsync = true;
            if (_options.ConnectionString != null)
                factory.Uri = new Uri(_options.ConnectionString);
            if (!string.IsNullOrEmpty(_options.HostName))
                factory.HostName = _options.HostName;
            if (!string.IsNullOrEmpty(_options.UserName))
                factory.HostName = _options.UserName;
            if (!string.IsNullOrEmpty(_options.Password))
                factory.HostName = _options.Password;
            if (!string.IsNullOrEmpty(_options.VirtualHost))
                factory.HostName = _options.VirtualHost;
            if (_options.Port != 0)
                factory.Port = _options.Port;
            _options.FactoryConfigure?.Invoke(factory);
            _connection = factory.CreateConnection();
            _connection.ConnectionBlocked += _connection_ConnectionBlocked;
            _connection.ConnectionUnblocked += _connection_ConnectionUnblocked;
            _connection.ConnectionShutdown += _connection_ConnectionShutdown;
            _connection.CallbackException += _connection_CallbackException;
            return Task.CompletedTask;
        }

        public override Task StopAsync()
        {
            if (_connection == null)
                return Task.CompletedTask;
            _connection.ConnectionBlocked -= _connection_ConnectionBlocked;
            _connection.ConnectionUnblocked -= _connection_ConnectionUnblocked;
            _connection.ConnectionShutdown -= _connection_ConnectionShutdown;
            _connection.CallbackException -= _connection_CallbackException;
            _connection.Close();
            _connection.Dispose();
            return Task.CompletedTask;
        }
    }
}
