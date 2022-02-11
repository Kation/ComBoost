using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;

namespace Wodsoft.ComBoost.Distributed.RabbitMQ
{
    public class DomainRabbitMQProvider : IDomainRabbitMQProvider, IDisposable
    {
        private DomainRabbitMQOptions _options;

        public DomainRabbitMQProvider(IOptions<DomainRabbitMQOptions> options)
        {
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        private IConnection? _connection;
        public IConnection GetConnection()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(DomainRabbitMQProvider));
            if (_connection == null)
            {
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
            }
            return _connection;
        }

        private bool _disposed;
        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                if (_connection != null)
                {
                    _connection.Dispose();
                    _connection = null;
                }
            }
        }
    }
}
