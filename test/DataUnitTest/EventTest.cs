using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost;
using Wodsoft.ComBoost.Mock;
using Xunit;

namespace DataUnitTest
{
    public class EventTest
    {
        [Fact]
        public async Task SyncEventTest()
        {
            var env = new EventTestEnvironment();
            await env.Run(async sp =>
            {
                MockController controller = new MockController(sp);
                var result = await controller.ExecuteAsync<EventDomainService, double>(context =>
                {
                    context.ValueProvider.SetValue("value", 1.0);
                }, "SyncEventTest");
                Assert.Equal(4, result);
            });
            await env.Run(async sp =>
            {
                MockController controller = new MockController(sp);
                var result = await controller.ExecuteAsync<EventDomainService, double>(context =>
                {
                    context.ValueProvider.SetValue("value", 2.0);
                    context.EventManager.AddEventHandler<OperatorEventArgs>(EventDomainService.SyncOperatorEvent, (ec, e) =>
                    {
                        e.Value *= 2;
                    });
                }, "SyncEventTest");
                Assert.Equal(7, result);
            });
            await env.Run(async sp =>
            {
                MockController controller = new MockController(sp);
                var result = await controller.ExecuteAsync<EventDomainService, double>(context =>
                {
                    context.ValueProvider.SetValue("value", 2.0);
                }, "SyncEventTest");
                Assert.Equal(5, result);
            });
        }
    }
}
