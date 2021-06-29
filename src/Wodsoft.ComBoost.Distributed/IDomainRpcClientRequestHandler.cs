using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost
{
    public interface IDomainRpcClientRequestHandler
    {
        void Handle(IDomainRpcRequest request, IDomainContext context);
    }
}
