using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Test;
using Xunit;

namespace Wodsoft.ComBoost.AspNetCore.Test
{
    public class AspNetCoreTest
    {
        [Fact]
        public async Task Greeter_SayHi_Test()
        {
            using var host = await new HostBuilder()
                .ConfigureWebHost(webBuilder =>
                {
                    webBuilder
                        .UseEnvironment(Environments.Development)
                        .ConfigureLogging(builder => builder.AddDebug())
                        .UseTestServer()
                        .UseStartup<Startup>();
                })
                .StartAsync();
            var client = host.GetTestClient();

            //Wodsoft.ComBoost.Grpc.AspNetCore.DomainGrpcService.GetAssembly();
            //var generator = new Lokad.ILPack.AssemblyGenerator();
            //// for ad-hoc serialization
            //var bytes = generator.GenerateAssemblyBytes(Wodsoft.ComBoost.Grpc.AspNetCore.DomainGrpcService.GetAssembly());
            //System.IO.File.WriteAllBytes("dynamic.dll", bytes);

            var responseText = await client.GetStringAsync("/greeterservice/hello?text=123");
            Assert.Equal("\"Hi.\"", responseText);
        }

        [Fact]
        public async Task DomainEndpoint_GetString()
        {
            using var host = await new HostBuilder()
                .ConfigureWebHost(webBuilder =>
                {
                    webBuilder
                        .UseEnvironment(Environments.Development)
                        .ConfigureLogging(builder => builder.AddDebug())
                        .UseTestServer()
                        .UseStartup<Startup>();
                })
                .StartAsync();
            var client = host.GetTestClient();
            var responseText = await client.GetStringAsync("/api/test/getstring?id=123");
            Assert.Equal(JsonSerializer.Serialize(new TestObject
            {
                Id = 123,
                Value = "Test"
            }, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }), responseText);
        }

        [Fact]
        public async Task DomainEndpoint_Api_GetString()
        {
            using var host = await new HostBuilder()
                .ConfigureWebHost(webBuilder =>
                {
                    webBuilder
                        .UseEnvironment(Environments.Development)
                        .ConfigureLogging(builder => builder.AddDebug())
                        .UseTestServer()
                        .UseStartup<Startup>();
                })
                .StartAsync();
            var client = host.GetTestClient();
            var responseText = await client.GetStringAsync("/api/testapi/getstring?id=123");
            Assert.Equal(JsonSerializer.Serialize(new ApiResult<TestObject>
            {
                Code = 0,
                Message = "Success",
                Content = new TestObject
                {
                    Id = 123,
                    Value = "Test"
                }
            }, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }), responseText);
        }
    }
}
