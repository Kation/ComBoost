using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost
{
    public interface IDomainRpcException
    {
        string Title { get; }

        string Message { get; }

        string Source { get; }

        string StackTrace { get; }

        IDomainRpcException InnerException { get; }
    }
}
