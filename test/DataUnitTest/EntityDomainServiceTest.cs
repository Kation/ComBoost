using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wodsoft.ComBoost.Mock;
using Wodsoft.ComBoost.Data.Entity;
using Wodsoft.ComBoost.Security;
using Wodsoft.ComBoost.Data;
using System.ComponentModel;
using Xunit;

namespace DataUnitTest
{
    public class EntityDomainServiceTest
    {
        [Fact]
        public async Task List()
        {
            var env = new UnitTestEnvironment();
            await env.Run(async sp =>
            {
                var databaseContext = sp.GetRequiredService<IDatabaseContext>();
                await DataCommon.DataInitAsync(databaseContext);
            });
            Guid id = Guid.Empty;
            string username = null;
            await env.Run(async sp =>
            {
                MockController controller = new MockController(sp);
                var model = await controller.ExecuteAsync<EntityDomainService<User>, IEntityViewModel<User>>("List");
                Assert.Equal(2, model.Items.Length);
                id = model.Items[0].Index;
                username = model.Items[0].Username;
            });
            await env.Run(async sp =>
            {
                MockController controller = new MockController(sp);
                var model = await controller.ExecuteAsync<EntityDomainService<User>, IEntityEditModel<User>>(context =>
                {
                    context.ValueProvider.SetValue("id", id);
                }, "Detail");
                Assert.Equal(username, model.Item.Username);
            });
        }
        
        [Fact]
        public async Task List2()
        {
            var env = new UnitTestEnvironment();
            await env.Run(async sp =>
            {
                MockController controller = new MockController(sp);
                await controller.ExecuteAsync<DataCommonService>("DataInit");
            });
            Guid id;
            string username;
            await env.Run(async sp =>
            {
                MockController controller = new MockController(sp);
                var model = await controller.ExecuteAsync<EntityDomainService<User>, IEntityViewModel<User>>("List");
                Assert.Equal(2, model.Items.Length);
                id = model.Items[0].Index;
                username = model.Items[0].Username;

                var category = await model.Items[0].LoadAsync(t => t.Category);
                var userQueryable = await category.LoadAsync(t => t.Users);
                var users = userQueryable.ToArrayAsync();
            });
        }
    }
}
