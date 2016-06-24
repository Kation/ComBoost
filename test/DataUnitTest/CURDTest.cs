using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DataUnitTest
{
    public class CURDTest
    {
        [Fact]
        public async Task AddAndRemoveTest()
        {
            var serviceProvider = DataCommon.GetServiceProvider();
            var database = serviceProvider.GetService<IDatabaseContext>();
            var categoryContext = database.GetContext<Category>();
            Assert.Equal(0, await categoryContext.CountAsync(categoryContext.Query()));
            var category = categoryContext.Create();
            category.Name = "Test";
            categoryContext.Add(category);
            Assert.Equal(0, await categoryContext.CountAsync(categoryContext.Query()));
            await database.SaveAsync();
            Assert.Equal(1, await categoryContext.CountAsync(categoryContext.Query()));
            categoryContext.Remove(category);
            Assert.Equal(1, await categoryContext.CountAsync(categoryContext.Query()));
            await database.SaveAsync();
            Assert.Equal(0, await categoryContext.CountAsync(categoryContext.Query()));
        }

        [Fact]
        public async Task LazyLoadEntityTest()
        {
            var serviceProvider = DataCommon.GetServiceProvider();
            var database = serviceProvider.GetService<IDatabaseContext>();
            var categoryContext = database.GetContext<Category>();
            var category = categoryContext.Create();
            category.Name = "Test";
            categoryContext.Add(category);
            var userContext = database.GetContext<User>();
            var user = userContext.Create();
            user.Username = "TestUser";
            user.Category = category;
            userContext.Add(user);
            await database.SaveAsync();

            database = serviceProvider.GetService<IDatabaseContext>();
            userContext = database.GetContext<User>();
            user = (await userContext.ToArrayAsync(userContext.Query()))[0];
            Assert.Null(user.Category);
            category = await user.LazyLoadEntityAsync(t => t.Category);
            Assert.NotNull(category);
            Assert.Equal("Test", category.Name);
        }

        [Fact]
        public async Task LazyLoadQueryTest()
        {
            var serviceProvider = DataCommon.GetServiceProvider();
            var database = serviceProvider.GetService<IDatabaseContext>();
            var categoryContext = database.GetContext<Category>();
            var category = categoryContext.Create();
            category.Name = "Test";
            categoryContext.Add(category);
            var userContext = database.GetContext<User>();
            var user = userContext.Create();
            user.Username = "TestUser";
            user.Category = category;
            userContext.Add(user);
            await database.SaveAsync();

            database = serviceProvider.GetService<IDatabaseContext>();
            categoryContext = database.GetContext<Category>();
            category = (await categoryContext.ToArrayAsync(categoryContext.Query()))[0];
            userContext = database.GetContext<User>();
            Assert.Null(category.Users);
            var collection = await category.LazyLoadCollectionAsync(t => t.Users);
            Assert.Equal(1, collection.Count);
            var users = await userContext.ToArrayAsync(collection);
            Assert.Equal("TestUser", users[0].Username);
        }
    }
}
