using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost
{
    public interface IDomainRpcServerRequestHandler
    {
        void Handle(IDomainRpcRequest request, DomainRpcContext context);
    }
}
