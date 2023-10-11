using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Mvc.Test.Services
{
    [DomainTemplateImplementer(typeof(ITestDomainService))]
    public class TestDomainService : DomainService
    {
        public Task<TestObject> GetString(int id)
        {
            return Task.FromResult(new TestObject { Id = id, Value = "Test" });
        }

        public Task CreateString(TestObject value)
        {
            return Task.CompletedTask;
        }

        public Task EditString(TestObject value)
        {
            return Task.CompletedTask;
        }

        public Task RemoveString(TestObject value)
        {
            return Task.CompletedTask;
        }

        public Task<bool?> HasValue(int id)
        {
            return Task.FromResult<bool?>(true);
        }
    }

    public interface ITestDomainService : IDomainTemplate
    {
        Task<TestObject> GetString(int id);

        Task CreateString(TestObject value);

        Task EditString(TestObject value, [FromQuery, Required] int id);

        Task RemoveString(TestObject value);

        Task<bool?> HasValue(int id);
    }
}
