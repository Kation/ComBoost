using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    public interface IDomainRpcServerResponseHandler
    {
        Task HandleAsync(IDomainRpcResponse response);
    }
}
