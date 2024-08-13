using AutoMapper.Extensions.ExpressionMapping;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Aggregation.Test.Entities;
using Wodsoft.ComBoost.Aggregation.Test.Models;
using Wodsoft.ComBoost.Data.Entity;
using Wodsoft.ComBoost.Mock;
using Xunit;
using Xunit.Abstractions;

[assembly: CollectionBehavior(CollectionBehavior.CollectionPerClass, DisableTestParallelization = true)]
namespace Wodsoft.ComBoost.Aggregation.Test
{
    [Collection("MvcTest")]
    public class MvcTest
    {
        private readonly ITestOutputHelper _output;

        public MvcTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task JsonTest()
        {
            IHost orgServiceMock = null;

            orgServiceMock = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddDbContext<OrganizationDataContext>(options => options.UseInMemoryDatabase("MvcOrganizationDataContext"));
                    services.AddEFCoreContext<OrganizationDataContext>();
                    services.AddComBoost()
                        .AddLocalService(builder =>
                        {
                            builder.AddAggregatorService<OrganizationEntity, Organization>();
                        })
                        .AddMock();
                    services.AddAutoMapper(config =>
                    {
                        config.AddExpressionMapping();

                        config.CreateMap<OrganizationEntity, Organization>();
                    });
                })
                .Build();

            using var host = await new HostBuilder()
                .ConfigureWebHost(webBuilder =>
                {
                    webBuilder
                        .UseEnvironment(Environments.Development)
                        .ConfigureLogging(builder => builder.AddDebug())
                        .UseTestServer()
                        .ConfigureServices(services =>
                        {
                            services.AddControllers();
                            services.AddDbContext<UserDataContext>(options => options.UseInMemoryDatabase("MvcUserDataContext"));
                            services.AddEFCoreContext<UserDataContext>();
                            services.AddComBoost()
                                .AddLocalService(builder =>
                                {
                                    builder.AddAggregation()
                                        .UseMemoryCache()
                                        .UseAggregatorService()
                                        .AddJsonExtension();
                                })
                                .AddMockService(() => orgServiceMock, builder =>
                                {
                                    builder.AddService<IDomainAggregatorService<Organization>>();
                                })
                                .AddMock();
                            services.AddAutoMapper(config =>
                            {
                                config.AddExpressionMapping();

                                config.CreateMap<UserEntity, User>();
                            });
                        })
                        .Configure(app =>
                        {
                            app.UseRouting();

                            app.UseEndpoints(endpoint =>
                            {
                                endpoint.MapDefaultControllerRoute();
                            });
                        });
                })
                .StartAsync();

            Guid rootOrgId = Guid.Empty;
            Guid subOrgId = Guid.Empty;

            await orgServiceMock.RunAsync(async sp =>
            {
                var entityContext = sp.GetRequiredService<IEntityContext<OrganizationEntity>>();
                var rootEntity = entityContext.Create();
                rootEntity.Name = "RootOrganization";
                entityContext.Add(rootEntity);
                var subEntity = entityContext.Create();
                subEntity.Name = "SubOrganization";
                subEntity.Parent = rootEntity;
                entityContext.Add(subEntity);
                await entityContext.Database.SaveAsync();
                rootOrgId = rootEntity.Id;
                subOrgId = subEntity.Id;
            });

            using (var scope = host.Services.CreateScope())
            {
                var entityContext = scope.ServiceProvider.GetRequiredService<IEntityContext<UserEntity>>();
                var entity = entityContext.Create();
                entity.UserName = "testUser";
                entity.DisplayName = "Test User";
                entity.OrganizationId = subOrgId;
                entityContext.Add(entity);
                await entityContext.Database.SaveAsync();
            }

            var client = host.GetTestClient();
            var result = await client.GetStringAsync("user");
            _output.WriteLine(result);
            var doc = JsonDocument.Parse(result);
            Assert.Equal(1, doc.RootElement.GetArrayLength());
            Assert.Throws<KeyNotFoundException>(() => doc.RootElement[0].GetProperty("organizationId"));
            var orgElement = doc.RootElement[0].GetProperty("organization");
            Assert.Equal(subOrgId, orgElement.GetProperty("id").GetGuid());
            var parentOrgElement = orgElement.GetProperty("parent");
            Assert.Equal(rootOrgId, parentOrgElement.GetProperty("id").GetGuid());
            Assert.Equal(JsonValueKind.Null, parentOrgElement.GetProperty("parent").ValueKind);
        }
    }
}
