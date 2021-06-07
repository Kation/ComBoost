using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost
{
    public interface IDomainRpcFilter
    {
        void OnRequest(IDomainRpcRequest request);

        void OnResponse(IDomainRpcResponse response);
    }
}
