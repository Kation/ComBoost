using Google.Protobuf;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using Wodsoft.Protobuf;

namespace Wodsoft.ComBoost.Grpc
{
    public class DomainGrpcMethodProtobufBuilder : IDomainGrpcMethodBuilder
    {
        public Method<TRequest, TResponse> CreateMethod<TRequest, TResponse>(string serviceName, string methodName)
        {
            return new Method<TRequest, TResponse>(MethodType.Unary, serviceName, methodName, new Marshaller<TRequest>((request) =>
            {
                try
                {

                    MemoryStream stream = new MemoryStream();
                    Message.Serialize(stream, request);
                    return stream.ToArray();
                }
                catch (Exception ex)
                {
                    throw new SerializationException($"Fail to serialize \"{typeof(TRequest).FullName}\".", ex);
                }
            }, (data) =>
            {
                try
                {
                    var input = new CodedInputStream(data);
                    var value = Message.Deserialize<TRequest>(input);
                    return value;
                }
                catch (Exception ex)
                {
                    throw new SerializationException($"Fail to deserialize \"{typeof(TRequest).FullName}\".", ex);
                }
            }), new Marshaller<TResponse>((response) =>
            {
                try
                {
                    MemoryStream stream = new MemoryStream();
                    Message.Serialize(stream, response);
                    return stream.ToArray();
                }
                catch (Exception ex)
                {
                    throw new SerializationException($"Fail to serialize \"{typeof(TResponse).FullName}\".", ex);
                }
            }, (data) =>
            {
                try
                {
                    var input = new CodedInputStream(data);
                    var value = Message.Deserialize<TResponse>(input);
                    return value;
                }
                catch (Exception ex)
                {
                    throw new SerializationException($"Fail to deserialize \"{typeof(TResponse).FullName}\".", ex);
                }
            }));
        }
    }
}
