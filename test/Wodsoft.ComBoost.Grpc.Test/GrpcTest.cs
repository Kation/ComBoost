using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Authentication;
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
using Wodsoft.ComBoost.Grpc.Test.Services;
using Wodsoft.ComBoost.Test;
using Wodsoft.ComBoost.Mock;
using Xunit;
using System.Linq;
using Microsoft.AspNetCore.Builder;

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
                    builder.AddAuthenticationPassthrough();
                    builder.AddService(host.GetTestServer().BaseAddress, new GrpcChannelOptions
                    {
                        HttpClient = client
                    }).UseTemplate<IGreeterTemplate>();
                })
                .AddMock(builder=>
                {

                });

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

        [Fact]
        public async Task AuthenticationTest()
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
                        .ConfigureServices(services =>
                        {
                            services.AddGrpc();
                            services.AddComBoost()
                                .AddLocalService(builder =>
                                {
                                    builder.AddService<AuthenticationTestService>();
                                })
                                .AddAspNetCore(builder =>
                                {
                                    builder.AddGrpcServices()
                                        .AddTemplate<IAuthenticationTestService>()
                                        .AddAuthenticationPassthrough();
                                });
                        })
                        .Configure(app =>
                        {
                            app.UseRouting();

                            app.UseEndpoints(endpoints =>
                            {
                                endpoints.MapDomainGrpcService();
                            });
                        });
                })
                .StartAsync();
            var client = host.GetTestClient();

            ServiceCollection services = new ServiceCollection();
            services.AddComBoost()
                .AddGrpcService(builder =>
                {
                    builder.AddAuthenticationPassthrough();
                    builder.AddService(host.GetTestServer().BaseAddress, new GrpcChannelOptions
                    {
                        HttpClient = client
                    }).UseTemplate<IAuthenticationTestService>();
                })
                .AddMock(builder =>
                {

                });

            var serviceProvider = services.BuildServiceProvider();
            using (var scope = serviceProvider.CreateScope())
            {
                scope.ServiceProvider.SetIdentity("test", "test", "a", "b", "c");
                var service = scope.ServiceProvider.GetRequiredService<IAuthenticationTestService>();
                var roles = await service.GetRoles();
                Assert.Equal(3, roles.Length);
            }
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
