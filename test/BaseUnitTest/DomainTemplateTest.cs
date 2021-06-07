using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Wodsoft.ComBoost;

namespace BaseUnitTest
{
    [TestClass]
    public class DomainTemplateTest
    {
        [TestMethod]
        public async Task LocalSerivceTest()
        {
            ServiceCollection services = new ServiceCollection();
            services.AddComBoost()
                .AddLocalService(builder =>
                {
                    builder.AddService<TestDomainService>().UseTemplate<ITestTemplate>();
                })
                .AddEmptyContextProvider();

            var serviceProvider = services.BuildServiceProvider();

            using (var scope = serviceProvider.CreateScope())
            {
                var testTemplate = scope.ServiceProvider.GetRequiredService<ITestTemplate>();
                var value = await testTemplate.Plus(2, 3);
                Assert.AreEqual(5, value);
            }
        }

        [DomainServiceTemplateDescriptor(typeof(ITestTemplate))]
        public class TestDomainService : DomainService
        {
            public Task<int> Plus([FromService] IValueProvider valueProvider, [FromValue] int left, [FromValue] int right)
            {
                return Task.FromResult(left + right);
            }
        }

        public interface ITestTemplate : IDomainTemplate
        {
            Task<int> Plus(int left, int right);
        }
    }
}
