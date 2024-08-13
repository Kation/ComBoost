using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Grpc.Test.Services
{
    public class GenericTestService<T> : DomainService
    {
        public Task<string> Test()
        {
            return Task.FromResult("123");
        }
    }
}
