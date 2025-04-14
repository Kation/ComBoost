using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Grpc.Test.Services
{
    public interface IGenericTestService<T> : IDomainTemplate
    {
        Task<string> NoRequest();

        Task NoResponse(string value);
    }
}
