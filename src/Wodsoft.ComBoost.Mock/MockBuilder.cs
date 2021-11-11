using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost.Mock
{
    public class MockBuilder : IMockBuilder
    {
        private IServiceCollection _services = new ServiceCollection();
        private List<Action<IConfiguration, IServiceCollection>> _serviceConfigures = new List<Action<IConfiguration, IServiceCollection>>();
        private ConfigurationBuilder _configBuilder = new ConfigurationBuilder();
        private IConfiguration _config;

        public IDictionary<string, object> Properties { get; } = new Dictionary<string, object>();

        public MockBuilder()
        {
            _services.AddSingleton(sp => _config);
        }

        public IMockBuilder ConfigureServices(Action<IConfiguration, IServiceCollection> serviceConfigure)
        {
            _serviceConfigures.Add(serviceConfigure ?? throw new ArgumentException(nameof(serviceConfigure)));
            return this;
        }

        [Obsolete]
        public IMock Build()
        {
            _config = _configBuilder.Build();
            foreach (var item in _serviceConfigures)
                item(_config, _services);
            return new Mock(_services.BuildServiceProvider());
        }

        public IMockBuilder ConfigureConfiguration(Action<IConfigurationBuilder> configureDelegate)
        {
            if (configureDelegate == null)
                throw new ArgumentNullException(nameof(configureDelegate));
            configureDelegate(_configBuilder);
            return this;
        }

        public IMockBuilder ConfigureLogging(Action<ILoggingBuilder> configureLogging)
        {
            _services.AddLogging(configureLogging);
            return this;
        }
    }
}
