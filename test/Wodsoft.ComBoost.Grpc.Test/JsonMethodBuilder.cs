using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Grpc.Test
{
    public class JsonMethodBuilder : IDomainGrpcMethodBuilder
    {
        public Method<TRequest, TResponse> CreateMethod<TRequest, TResponse>(string serviceName, string methodName)
        {
            return new Method<TRequest, TResponse>(MethodType.Unary, serviceName, methodName, new Marshaller<TRequest>(request =>
            {
                return JsonSerializer.SerializeToUtf8Bytes(request);
            }, data =>
            {
                return JsonSerializer.Deserialize<TRequest>(data);
            }), new Marshaller<TResponse>(response =>
            {
                return JsonSerializer.SerializeToUtf8Bytes(response);
            }, data =>
            {
                return JsonSerializer.Deserialize<TResponse>(data);
            }));
        }
    }
}
