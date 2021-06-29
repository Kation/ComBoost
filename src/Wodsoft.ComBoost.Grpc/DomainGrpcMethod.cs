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
        where TRequest : class, IMessage, new()
        where TResponse : class, IMessage, new()
    {
        public static Method<TRequest, TResponse> CreateMethod(string serviceName, string methodName)
        {
            return new Method<TRequest, TResponse>(MethodType.Unary, serviceName, methodName, new Marshaller<TRequest>((request) =>
            {
                MemoryStream stream = new MemoryStream();
                var output = new CodedOutputStream(stream, true);
                output.WriteRawMessage(request);
                output.Flush();
                return stream.ToArray();
            }, (data) =>
            {
                var input = new CodedInputStream(data);
                var value = new TRequest();
                input.ReadRawMessage(value);
                return value;
            }), new Marshaller<TResponse>((response) =>
            {
                MemoryStream stream = new MemoryStream();
                var output = new CodedOutputStream(stream, true);
                output.WriteRawMessage(response);
                output.Flush();
                return stream.ToArray();
            }, (data) =>
            {
                var input = new CodedInputStream(data);
                var value = new TResponse();
                input.ReadRawMessage(value);
                return value;
            }));
        }
    }
}
