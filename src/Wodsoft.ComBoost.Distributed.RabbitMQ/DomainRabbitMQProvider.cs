using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;

namespace Wodsoft.ComBoost.Distributed.RabbitMQ
{
    public class DomainRabbitMQProvider : IDomainRabbitMQProvider, IDisposable, IHealthStateProvider
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
                    factory.UserName = _options.UserName;
                if (!string.IsNullOrEmpty(_options.Password))
                    factory.Password = _options.Password;
                if (!string.IsNullOrEmpty(_options.VirtualHost))
                    factory.VirtualHost = _options.VirtualHost;
                if (_options.Port != 0)
                    factory.Port = _options.Port;
                _options.FactoryConfigure?.Invoke(factory);
                _connection = factory.CreateConnection();
                _connection.ConnectionBlocked += connection_ConnectionBlocked;
                _connection.ConnectionUnblocked += connection_ConnectionUnblocked;
                _connection.ConnectionShutdown += connection_ConnectionShutdown;
                if (_connection is IAutorecoveringConnection autorecovering)
                {
                    autorecovering.RecoverySucceeded += connection_RecoverySucceeded;
                }
            }
            return _connection;
        }

        private void connection_RecoverySucceeded(object sender, EventArgs e)
        {
            State = HealthState.Healthy;
        }

        private void connection_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            State = _options.IsCriticalHealthState ? HealthState.Critical : HealthState.Warning;
        }

        private void connection_ConnectionUnblocked(object sender, EventArgs e)
        {
            State = HealthState.Healthy;
        }

        private void connection_ConnectionBlocked(object sender, global::RabbitMQ.Client.Events.ConnectionBlockedEventArgs e)
        {
            State = _options.IsCriticalHealthState ? HealthState.Critical : HealthState.Warning;
        }

        private bool _disposed;

        public event EventHandler<HealthStateChangedEventArgs>? HealthStateChanged;

        private volatile HealthState _state;
        public HealthState State
        {
            get => _state; set
            {
                if (_state != value)
                {
                    _state = value;
                    HealthStateChanged?.Invoke(this, new HealthStateChangedEventArgs(value));
                }
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                if (_connection != null)
                {
                    _connection.ConnectionBlocked -= connection_ConnectionBlocked;
                    _connection.ConnectionUnblocked -= connection_ConnectionUnblocked;
                    _connection.ConnectionShutdown -= connection_ConnectionShutdown;
                    if (_connection is IAutorecoveringConnection autorecovering)
                    {
                        autorecovering.RecoverySucceeded -= connection_RecoverySucceeded;
                    }
                    _connection.Dispose();
                    _connection = null;
                }
            }
        }
    }
}
