//using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity;
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
            Category category = null;
            await env.Run(async sp =>
            {
                var context = (DataContext)sp.GetRequiredService<Microsoft.EntityFrameworkCore.DbContext>();

                category = new Category { Index = Guid.NewGuid(), CreateDate = DateTime.Now, EditDate = DateTime.Now, Name = "Test" };
                context.Category.Add(category);
                await context.SaveChangesAsync();
            });
            await env.Run(async sp =>
            {
                var databaseContext = sp.GetService<Wodsoft.ComBoost.Data.Entity.IDatabaseContext>();
                var context = (DataContext)((Wodsoft.ComBoost.Data.Entity.DatabaseContext)databaseContext).InnerContext;
                Assert.Equal(1, await context.Category.CountAsync());
                var categoryContext = databaseContext.GetContext<Category>();
                var category2 = await categoryContext.GetAsync(category.Index);

                var user = new User { Index = Guid.NewGuid(), Category = category2, CreateDate = DateTime.Now, EditDate = DateTime.Now, Username = "Test1" };
                databaseContext.GetContext<User>().Add(user);
                Assert.Equal(Microsoft.EntityFrameworkCore.EntityState.Unchanged, context.Entry(category2).State);
                //category2.Name = "Changed";
                await context.SaveChangesAsync();
            });
            await env.Run(async sp =>
            {
                var databaseContext = sp.GetService<Wodsoft.ComBoost.Data.Entity.IDatabaseContext>();
                var categoryContext = databaseContext.GetContext<Category>();
                var category3 = await categoryContext.GetAsync(category.Index);
                Assert.Equal("Test", category3.Name);

                var users = await category3.LoadAsync(t => t.Users);
                var user = new User { Index = Guid.NewGuid(), CreateDate = DateTime.Now, EditDate = DateTime.Now, Username = "Test1" };
                users.Add(user);
                Assert.Equal(category3, user.Category);
            });
        }
    }
}
