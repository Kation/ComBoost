using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Aggregation.Test.Entities;
using Wodsoft.ComBoost.Aggregation.Test.Models;
using Wodsoft.ComBoost.Data;
using Wodsoft.ComBoost.Data.Entity;
using Wodsoft.ComBoost.Mock;
using Xunit;

namespace Wodsoft.ComBoost.Aggregation.Test
{
    [Collection("AggregationTest")]
    public class AggregationTest
    {
        [Fact]
        public void ExportDll()
        {
            var userType = DomainAggregationsBuilder<User>.AggregationType;
            var orgType = DomainAggregationsBuilder<Organization>.AggregationType;

            var generator = new Lokad.ILPack.AssemblyGenerator();
            var bytes = generator.GenerateAssemblyBytes(DomainAggregationsBuilder.Module.Assembly);
            System.IO.File.WriteAllBytes("dynamic.dll", bytes);
        }

        [Fact]
        public async Task User_Organization_Test()
        {
            IHost userServiceMock = null, orgServiceMock = null;

            userServiceMock = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddDbContext<UserDataContext>(options => options.UseInMemoryDatabase("UserDataContext"));
                    services.AddEFCoreContext<UserDataContext>();
                    services.AddComBoost()
                        .AddLocalService(builder =>
                        {
                            builder.AddAggregation()
                                .UseMemoryCache()
                                .UseAggregatorService();
                        })
                        .AddMockService(()=> orgServiceMock, builder=>
                        {
                            builder.AddService<IDomainAggregatorService<Organization, Guid>>();
                        })
                        .AddMock();
                    services.AddAutoMapper(config =>
                    {
                        config.CreateMap<UserEntity, User>();
                    });
                })
                .Build();

            orgServiceMock = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddDbContext<OrganizationDataContext>(options => options.UseInMemoryDatabase("OrganizationDataContext"));
                    services.AddEFCoreContext<OrganizationDataContext>();
                    services.AddComBoost()
                        .AddLocalService(builder =>
                        {
                            builder.AddAggregatorService<Guid, OrganizationEntity, Organization>();
                        })
                        .AddMock();
                    services.AddAutoMapper(config =>
                    {
                        config.CreateMap<OrganizationEntity, Organization>();
                    });
                })
                .Build();

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

            await userServiceMock.RunAsync(async sp =>
            {
                var entityContext = sp.GetRequiredService<IEntityContext<UserEntity>>();
                var entity = entityContext.Create();
                entity.UserName = "testUser";
                entity.DisplayName = "Test User";
                entity.OrganizationId = subOrgId;
                entityContext.Add(entity);
                await entityContext.Database.SaveAsync();
            });

            await userServiceMock.RunAsync(async sp =>
            {
                var mapper = sp.GetRequiredService<IMapper>();
                var aggregator = sp.GetRequiredService<IDomainAggregator>();
                var entityContext = sp.GetRequiredService<IEntityContext<UserEntity>>();
                var user = await entityContext.Query().ProjectTo<User>(mapper.ConfigurationProvider).FirstOrDefaultAsync();
                Assert.NotNull(user);
                var userAggregation = await aggregator.AggregateAsync(user);
            });

            await userServiceMock.RunAsync(async sp =>
            {
                var mapper = sp.GetRequiredService<IMapper>();
                var aggregator = sp.GetRequiredService<IDomainAggregator>();
                var entityContext = sp.GetRequiredService<IEntityContext<UserEntity>>();
                var user = await entityContext.Query().ProjectTo<User>(mapper.ConfigurationProvider).FirstOrDefaultAsync();
                Assert.NotNull(user);
                var userAggregation = await aggregator.AggregateAsync(user);
            });

            await Task.Delay(10000);
        }
    }
}
