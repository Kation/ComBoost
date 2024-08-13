using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost
{
    public interface IDomainRpcServerResponseHandler
    {
        void Handle(IDomainRpcResponse response);
    }
}
