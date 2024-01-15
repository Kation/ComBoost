using Microsoft.Data.Sqlite;
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
        private SqliteConnection _connection;

        private void CreateConnection()
        {
            _connection = new SqliteConnection("Filename=:memory:");
            _connection.Open();
        }

        [Fact]
        public async Task CURDTest()
        {
            CreateConnection();
            var mock = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddDbContext<DataContext>(options => options.UseSqlite(_connection));
                    services.AddEFCoreContext<DataContext>();
                    services.AddComBoost()
                        .AddLocalService(builder =>
                        {
                            builder.AddEntityService<UserEntity, UserDto>();
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

            mock.Run(sp =>
            {
                sp.GetRequiredService<DataContext>().Database.EnsureCreated();
            });

            await mock.RunAsync(async sp =>
            {
                var template = sp.GetRequiredService<IEntityDomainTemplate<UserDto>>();
                //var generator = new Lokad.ILPack.AssemblyGenerator();
                //var bytes = generator.GenerateAssemblyBytes(DomainTemplateBuilder.Module.Assembly);
                //File.WriteAllBytes("dynamic.dll", bytes);
                var viewModel = await template.List();
                Assert.Empty(viewModel.Items);
            });

            UserDto user = null;

            await mock.RunAsync(async sp =>
            {
                var template = sp.GetRequiredService<IEntityDomainTemplate<UserDto>>();
                var model = await template.Create(new UserDto
                {
                    UserName = "test1",
                    Email = "test1@test.com",
                    DisplayName = "Test Account 1",
                    Password = "123456"
                });
                Assert.True(model.IsSuccess);
                Assert.NotNull(model.Item);
                Assert.NotEqual(Guid.Empty, model.Item.Id);
                Assert.Equal(model.Item.CreationDate, model.Item.ModificationDate);
                user = model.Item;
            });

            await mock.RunAsync(async sp =>
            {
                var template = sp.GetRequiredService<IEntityDomainTemplate<UserDto>>();
                var viewModel = await template.List();
                Assert.Single(viewModel.Items);
            });

            user.DisplayName = "New Name";
            await mock.RunAsync(async sp =>
            {
                var template = sp.GetRequiredService<IEntityDomainTemplate<UserDto>>();
                var model = await template.Edit(user);
                Assert.True(model.IsSuccess);
                Assert.NotNull(model.Item);
                Assert.NotEqual(model.Item.CreationDate, model.Item.ModificationDate);
                Assert.Equal("New Name", model.Item.DisplayName);
            });

            await mock.RunAsync(async sp =>
            {
                var template = sp.GetRequiredService<IEntityDomainTemplate<UserDto>>();
                await template.Remove(user);
            });

            await mock.RunAsync(async sp =>
            {
                var template = sp.GetRequiredService<IEntityDomainTemplate<UserDto>>();
                var viewModel = await template.List();
                Assert.Empty(viewModel.Items);
            });
        }

        [Fact]
        public async Task ValidationTest()
        {
            CreateConnection();
            var mock = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddDbContext<DataContext>(options => options.UseSqlite(_connection));
                    services.AddEFCoreContext<DataContext>();
                    services.AddComBoost()
                        .AddLocalService(builder =>
                        {
                            builder.AddEntityService<UserEntity, UserDto>();
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
                var template = sp.GetRequiredService<IEntityDomainTemplate<UserDto>>();
                var model = await template.Create(new UserDto
                {
                    UserName = "test1",
                    Email = "12345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890",
                    Password = "123456"
                });
                Assert.False(model.IsSuccess);
                Assert.Contains(model.ErrorMessage, t => t.Key == nameof(UserDto.DisplayName));
                Assert.Contains(model.ErrorMessage, t => t.Key == nameof(UserDto.Email));
            });
        }

        [Fact]
        public async Task CancelCreateTest()
        {
            CreateConnection();
            var mock = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddDbContext<DataContext>(options => options.UseSqlite(_connection));
                    services.AddEFCoreContext<DataContext>();
                    services.AddComBoost()
                        .AddLocalService(builder =>
                        {
                            builder.AddEntityService<UserEntity, UserDto>();
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

            mock.Run(sp =>
            {
                sp.GetRequiredService<DataContext>().Database.EnsureCreated();
            });

            await mock.RunAsync(async sp =>
            {
                var template = sp.GetRequiredService<IEntityDomainTemplate<UserDto>>();
                var viewModel = await template.List();
                Assert.Empty(viewModel.Items);
            });

            await mock.RunAsync(async sp =>
            {
                var template = sp.GetRequiredService<IEntityDomainTemplate<UserDto>>();
                template.Context.EventManager.AddEventHandler<EntityPreCreateEventArgs<UserEntity>>((context, e) =>
                {
                    e.IsCanceled = true;
                    return Task.CompletedTask;
                });
                var model = await template.Create(new UserDto
                {
                    UserName = "test1",
                    Email = "test1@test.com",
                    DisplayName = "Test Account 1",
                    Password = "123456"
                });
                Assert.True(model.IsSuccess);
            });

            await mock.RunAsync(async sp =>
            {
                var template = sp.GetRequiredService<IEntityDomainTemplate<UserDto>>();
                var viewModel = await template.List();
                Assert.Empty(viewModel.Items);
            });
        }
    }
}
