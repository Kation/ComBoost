using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Grpc.Test.Services
{
    [DomainTemplateImplementer(typeof(IGenericModelTestService))]
    public class GenericModelTestService : DomainService
    {
        public Task<GenericModel<string>> Test()
        {
            return Task.FromResult(new GenericModel<string> { Model = "123" });
        }
    }
}
