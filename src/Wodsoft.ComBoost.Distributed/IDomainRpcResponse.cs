using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost
{
    public interface IDomainRpcResponse
    {
        string OS { get; }

        IDictionary<string, byte[]> Headers { get; }

        IDomainRpcTrace Trace { get; }

        IDomainRpcException Exception { get; }
    }

    public interface IDomainRpcResponse<T> : IDomainRpcResponse
    {
        T Result { get; }
    }
}
