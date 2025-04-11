using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    public interface IDomainRpcServerRequestHandler
    {
        Task HandleAsync(IDomainRpcRequest request, DomainRpcContext context);
    }
}
