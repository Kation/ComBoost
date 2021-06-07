using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost
{
    public interface IDomainRpcResponse
    {
        string OS { get; }

        IDomainRpcTrace Trace { get; }

        IDomainRpcException Exception { get; }
    }

    public interface IDomainRpcResponse<T> : IDomainRpcResponse
    {
        T Result { get; }
    }
}
