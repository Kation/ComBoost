using Grpc.Core;
using Grpc.Net.Client;
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
using System.Threading;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Test;
using Xunit;

namespace Wodsoft.ComBoost.Grpc.Test
{
    public class GrpcTest
    {
        [Fact]
        public async Task Greeter_SayHi_Test()
        {
            using var host = await new HostBuilder()
                .ConfigureWebHost(webBuilder =>
                {
                    webBuilder
                        .UseEnvironment(Environments.Development)
                        .ConfigureAppConfiguration((context, configBuilder) =>
                        {
                            configBuilder.AddJsonFile("appsettings.json", true);
                            configBuilder.AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", true);
                        })
                        .ConfigureLogging(builder => builder.AddDebug())
                        .UseTestServer()
                        .UseStartup<Startup>()
                        .ConfigureServices(services =>
                        {

                        });
                })
                .StartAsync();
            var client = host.GetTestClient();

            //Wodsoft.ComBoost.Grpc.AspNetCore.DomainGrpcService.GetAssembly();
            //var generator = new Lokad.ILPack.AssemblyGenerator();
            //// for ad-hoc serialization
            //var bytes = generator.GenerateAssemblyBytes(Wodsoft.ComBoost.Grpc.AspNetCore.DomainGrpcService.GetAssembly());
            //System.IO.File.WriteAllBytes("dynamic.dll", bytes);

            ServiceCollection services = new ServiceCollection();
            services.AddComBoost()
                .AddGrpcService(builder =>
                {
                    builder.AddService(host.GetTestServer().BaseAddress, new GrpcChannelOptions
                    {
                        HttpClient = client
                    }).UseTemplate<IGreeterTemplate>();
                })
                .AddEmptyContextProvider();

            var serviceProvider = services.BuildServiceProvider();
            using (var scope = serviceProvider.CreateScope())
            {
                var greeter = scope.ServiceProvider.GetRequiredService<IGreeterTemplate>();
                var request = new HelloRequest { Name = "Kation" };
                var response = await greeter.SayHi(request);
                Assert.Equal($"Hi {request.Name}.", response.Answer);
                Assert.Equal("Hi.", await greeter.Hello());
                Assert.Equal("Hi.", await greeter.Hello("I'm Kation."));
            }
        }

        private void Start()
        {
            Action<string, string> action = Test;
        }

        private void Test(string a, string b)
        {

        }

        private class ResponseVersionHandler : DelegatingHandler
        {
            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                CancellationToken cancellationToken)
            {
                var response = await base.SendAsync(request, cancellationToken);
                response.Version = request.Version;

                return response;
            }
        }
    }
}
