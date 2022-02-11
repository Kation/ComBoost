using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost.Mock
{
    public class MockEnvironment : IHostEnvironment
    {
        public MockEnvironment(IOptions<MockEnvironmentOptions> options)
        {
            EnvironmentName = options.Value.EnvironmentName;
        }

        public string? EnvironmentName { get; set; }
        public string? ApplicationName { get; set; }
        public string? ContentRootPath { get; set; }
        public IFileProvider? ContentRootFileProvider { get; set; }
    }
}
