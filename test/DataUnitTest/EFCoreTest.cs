using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using Wodsoft.ComBoost.Data.Entity;
using Xunit;
using Microsoft.Extensions.DependencyInjection;

namespace DataUnitTest
{
    public class EFCoreTest
    {
        [Fact]
        public async Task Test()
        {

            UnitTestEnvironment env = new UnitTestEnvironment();
            Category category;
            using (var scope = env.GetServiceScope())
            {
                var context = (DataContext)scope.ServiceProvider.GetRequiredService<DbContext>();

                category = new Category { Index = Guid.NewGuid(), CreateDate = DateTime.Now, EditDate = DateTime.Now, Name = "Test" };
                context.Category.Add(category);
                await context.SaveChangesAsync();
            }
            using (var scope = env.GetServiceScope())
            {
                var databaseContext = scope.ServiceProvider.GetService<Wodsoft.ComBoost.Data.Entity.IDatabaseContext>();
                var context = (DataContext)((Wodsoft.ComBoost.Data.Entity.DatabaseContext)databaseContext).InnerContext;
                Assert.Equal(1, await context.Category.CountAsync());
                var categoryContext = databaseContext.GetContext<Category>();
                var category2 = await categoryContext.GetAsync(category.Index);

                var user = new User { Index = Guid.NewGuid(), Category = category2, CreateDate = DateTime.Now, EditDate = DateTime.Now, Username = "Test" };
                databaseContext.GetContext<User>().Add(user);
                Assert.Equal(EntityState.Unchanged, context.Entry(category2).State);
                //category2.Name = "Changed";
                await context.SaveChangesAsync();
            }
            using (var scope = env.GetServiceScope())
            {
                var databaseContext = scope.ServiceProvider.GetService<Wodsoft.ComBoost.Data.Entity.IDatabaseContext>();
                var categoryContext = databaseContext.GetContext<Category>();
                var category3 = await categoryContext.GetAsync(category.Index);
                Assert.Equal("Test", category3.Name);
            }
        }
    }
}
