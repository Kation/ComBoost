using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Wodsoft.ComBoost.Grpc
{
    public interface IDomainGrpcEndPointProvider
    {
        void GetEndPoint(MethodInfo method, out string serviceName, out string methodName);
    }
}
