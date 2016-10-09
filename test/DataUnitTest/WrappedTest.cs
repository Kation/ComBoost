using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity;
using Xunit;

namespace DataUnitTest
{
    public class WrappedTest
    {
        [Fact]
        public async Task AddAndRemoveTest()
        {
            var serviceProvider = DataCommon.GetServiceProvider();
            var database = serviceProvider.GetService<IDatabaseContext>();
            var categoryContext = database.GetWrappedContext<ICategory>();
            var userContext = database.GetWrappedContext<IUser>();
            Assert.Equal(0, await categoryContext.CountAsync(categoryContext.Query()));
            var category = categoryContext.Create();
            category.Name = "Test";
            categoryContext.Add(category);
            await database.SaveAsync();

            var user = userContext.Create();
            user.Username = "TestUser";
            user.Category = category;
            userContext.Add(user);
            await database.SaveAsync();

            Assert.Equal(1, await userContext.CountAsync(userContext.Query().Where(t => t.Category.Name == "Test")));
        }
    }
}
