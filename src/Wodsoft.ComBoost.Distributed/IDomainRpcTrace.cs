using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost
{
    public interface IDomainRpcTrace
    {
        DateTimeOffset StartTime { get; }

        DateTimeOffset EndTime { get; }

        TimeSpan ElapsedTime { get; }

        IEnumerable<IDomainRpcTrace> InnerCall { get; }
    }
}
