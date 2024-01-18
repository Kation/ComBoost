using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Test
{
    //[DomainDistributedService("TestService")]
    public interface IGreeterTemplate : IDomainTemplate
    {
        [DomainValue("invokeType", InvokeType.Test)]
        Task<HelloResponse> SayHi(HelloRequest request);

        Task<string> Hello();

        Task<string> Hello(string text);

        Task Test();
        Task Test(Guid id);

        Task EventTest();
    }
}
