using Google.Protobuf;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Wodsoft.Protobuf;

namespace Wodsoft.ComBoost.Grpc
{
    public class DomainGrpcMethod<TRequest, TResponse>
        where TRequest : class, new()
        where TResponse : class, new()
    {
        public static Method<TRequest, TResponse> CreateMethod(string serviceName, string methodName)
        {
            return new Method<TRequest, TResponse>(MethodType.Unary, serviceName, methodName, new Marshaller<TRequest>((request) =>
            {
                MemoryStream stream = new MemoryStream();
                Message<TRequest>.Serialize(stream, request);
                return stream.ToArray();
            }, (data) =>
            {
                MemoryStream stream = new MemoryStream(data);
                return Message<TRequest>.Deserialize(stream);
            }), new Marshaller<TResponse>((response) =>
            {
                MemoryStream stream = new MemoryStream();
                Message<TResponse>.Serialize(stream, response);
                return stream.ToArray();
            }, (data) =>
            {
                MemoryStream stream = new MemoryStream(data);
                return Message<TResponse>.Deserialize(stream);
            }));
        }
    }
}
