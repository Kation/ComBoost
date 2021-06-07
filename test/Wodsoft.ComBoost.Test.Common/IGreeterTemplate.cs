using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Test
{
    public interface IGreeterTemplate : IDomainTemplate
    {
        Task<HelloResponse> SayHi(HelloRequest request);

        Task Hello();

        Task Hello(string text);
    }
}
