using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost
{
    public interface IDomainRpcClientResponseHandler
    {
        void Handle(IDomainRpcResponse response);
    }
}
