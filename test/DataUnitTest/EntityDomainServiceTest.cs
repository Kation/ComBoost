using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wodsoft.ComBoost.Mock;
using Microsoft.EntityFrameworkCore;
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
            using (var scope = env.GetServiceScope())
            {
                var databaseContext = scope.ServiceProvider.GetRequiredService<IDatabaseContext>();
                await DataCommon.DataInitAsync(databaseContext);
            }
            Guid id;
            string username;
            using (var scope = env.GetServiceScope())
            {
                MockController controller = new MockController(scope.ServiceProvider);
                var model = await controller.ExecuteAsync<EntityDomainService<User>, IEntityViewModel<User>>("List");
                Assert.Equal(2, model.Items.Length);
                id = model.Items[0].Index;
                username = model.Items[0].Username;
            }
            using (var scope = env.GetServiceScope())
            {
                MockController controller = new MockController(scope.ServiceProvider);
                var model = await controller.ExecuteAsync<EntityDomainService<User>, IEntityEditModel<User>>(context =>
                {
                    context.ValueProvider.SetValue("id", id);
                }, "Detail");
                Assert.Equal(username, model.Item.Username);
            }
        }
    }
}
