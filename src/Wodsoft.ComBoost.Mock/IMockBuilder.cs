using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost.Mock
{
    public interface IMockBuilder
    {
        IDictionary<string, object> Properties { get; }

        IMockBuilder ConfigureServices(Action<IConfiguration, IServiceCollection> serviceConfigure);

        IMockBuilder ConfigureConfiguration(Action<IConfigurationBuilder> configureDelegate);

        IMockBuilder ConfigureLogging(Action<ILoggingBuilder> configureLogging);

        [Obsolete]
        IMock Build();
    }
}
