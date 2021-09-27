using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Mock;
using Wodsoft.ComBoost.Test;
using Wodsoft.ComBoost.Test.Entities;
using Wodsoft.ComBoost.Test.Models;
using Xunit;

namespace Wodsoft.ComBoost.Data.Test
{
    public class EntityDomainServiceTest
    {
        [Fact]
        public async Task CURDTest()
        {
            var mock = Mock.Mock.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddDbContext<DataContext>(options => options.UseInMemoryDatabase("Wodsoft.ComBoost.Data.Test"));
                    services.AddEFCoreContext<DataContext>();
                    services.AddComBoost()
                        .AddLocalService(builder =>
                        {
                            builder.AddEntityService<Guid, UserEntity, UserDto>();
                        })
                        .AddMock();
                    services.AddAutoMapper(config =>
                    {
                        config.CreateMap<UserEntity, UserDto>()
                            .ForMember(t => t.Password, options => options.Ignore());
                        config.CreateMap<UserDto, UserEntity>()
                            .ForMember(t => t.Password, options => options.Ignore())
                            .AfterMap((dto, entity) =>
                            {
                                if (!string.IsNullOrEmpty(dto.Password))
                                    entity.SetPassword(dto.Password);
                            });
                    });
                })
                .Build();

            await mock.RunAsync(async sp =>
            {
                var template = sp.GetRequiredService<IEntityDomainTemplate<Guid, UserDto>>();
                //var generator = new Lokad.ILPack.AssemblyGenerator();
                //var bytes = generator.GenerateAssemblyBytes(DomainTemplateBuilder.Module.Assembly);
                //File.WriteAllBytes("dynamic.dll", bytes);
                var viewModel = await template.List();
                Assert.Empty(viewModel.Items);
            });

            UserDto user = null;

            await mock.RunAsync(async sp =>
            {
                var template = sp.GetRequiredService<IEntityDomainTemplate<Guid, UserDto>>();
                var model = await template.Create(new UserDto
                {
                    UserName = "test1",
                    Email = "test1@test.com",
                    DisplayName = "Test Account 1",
                    Password = "123456"
                });
                Assert.True(model.IsSuccess);
                Assert.NotNull(model.Result);
                Assert.NotEqual(Guid.Empty, model.Result.Id);
                Assert.Equal(model.Result.CreationDate, model.Result.ModificationDate);
                user = model.Result;
            });

            await mock.RunAsync(async sp =>
            {
                var template = sp.GetRequiredService<IEntityDomainTemplate<Guid, UserDto>>();
                var viewModel = await template.List();
                Assert.Single(viewModel.Items);
            });

            user.DisplayName = "New Name";
            await mock.RunAsync(async sp =>
            {
                var template = sp.GetRequiredService<IEntityDomainTemplate<Guid, UserDto>>();
                var model = await template.Edit(user);
                Assert.True(model.IsSuccess);
                Assert.NotNull(model.Result);
                Assert.NotEqual(model.Result.CreationDate, model.Result.ModificationDate);
                Assert.Equal("New Name", model.Result.DisplayName);
            });

            await mock.RunAsync(async sp =>
            {
                var template = sp.GetRequiredService<IEntityDomainTemplate<Guid, UserDto>>();
                await template.Remove(user.Id);
            });

            await mock.RunAsync(async sp =>
            {
                var template = sp.GetRequiredService<IEntityDomainTemplate<Guid, UserDto>>();
                var viewModel = await template.List();
                Assert.Empty(viewModel.Items);
            });
        }
    }
}
