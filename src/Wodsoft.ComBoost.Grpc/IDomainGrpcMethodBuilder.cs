using Google.Protobuf;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost.Grpc
{
    public interface IDomainGrpcMethodBuilder
    {
        Method<TRequest, TResponse> CreateMethod<TRequest, TResponse>(string serviceName, string methodName);
    }
}
