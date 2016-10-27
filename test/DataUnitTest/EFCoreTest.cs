using Microsoft.EntityFrameworkCore;
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
            var context = new DataContext(new DbContextOptionsBuilder<DataContext>().UseInMemoryDatabase().Options.WithExtension(new ComBoostOptionExtension()));

            var category = new Category { Index = Guid.NewGuid(), CreateDate = DateTime.Now, EditDate = DateTime.Now, Name = "Test" };
            context.Category.Add(category);
            await context.SaveChangesAsync();

            var databaseContext = DataCommon.GetServiceProvider().GetService<IDatabaseContext>();
            context = (DataContext)((DatabaseContext)databaseContext).InnerContext;
            Assert.Equal(1, await context.Category.CountAsync());
            var categoryContext = databaseContext.GetContext<Category>();
            var category2 = await categoryContext.GetAsync(category.Index);

            var user = new User { Index = Guid.NewGuid(), Category = category2, CreateDate = DateTime.Now, EditDate = DateTime.Now, Username = "Test" };
            databaseContext.GetContext<User>().Add(user);
            Assert.Equal(EntityState.Unchanged, context.Entry(category2).State);
            //category2.Name = "Changed";
            await context.SaveChangesAsync();

            databaseContext = DataCommon.GetServiceProvider().GetService<IDatabaseContext>();
            var category3 = await categoryContext.GetAsync(category.Index);
            Assert.Equal("Test", category3.Name);
        }
    }
}
