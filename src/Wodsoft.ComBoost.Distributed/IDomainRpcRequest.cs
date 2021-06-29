using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Wodsoft.ComBoost
{
    public interface IDomainRpcRequest
    {
        string OS { get; }

        IDictionary<string, byte[]> Headers { get; }

        IDictionary<string, string> Values { get; }
    }

    public interface DomainRpcRequest<T> : IDomainRpcRequest
    {
        T Parameters { get; }
    }
}
