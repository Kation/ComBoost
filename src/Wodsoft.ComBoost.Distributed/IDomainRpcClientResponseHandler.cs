using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    public interface IDomainRpcClientResponseHandler
    {
        Task HandleAsync(IDomainRpcResponse response);
    }
}
