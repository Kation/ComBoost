using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.Protobuf;
using Xunit;

namespace Wodsoft.ComBoost.Grpc.Test
{
    public class SerializationTest
    {
        [Fact]
        public void Simply_Class_Test()
        {
            //DomainGrpcArgument<Guid, string> argument = new DomainGrpcArgument<Guid, string>()
            //{
            //    Argument1 = new Guid("a089ee87-46b3-424a-8868-8584b9607881"),
            //    Argument2 = "AQAAAAEAACcQAAAAEKGHgbY43PS0yOoO3gU19ML1z1Mn+MG9pP7ok3WbWtyagqkOuBuVCnEarjuxubK97Q=="
            //};
            //DomainGrpcRequest<DomainGrpcArgument<Guid, string>> request = new DomainGrpcRequest<DomainGrpcArgument<Guid, string>>
            //{
            //    Argument = argument,
            //    OS = "Microsoft Windows NT 10.0.19042.0"
            //};
            //var data = method.RequestMarshaller.Serializer(request);
            //var data2 = Convert.FromHexString("0A214D6963726F736F66742057696E646F7773204E542031302E302E31393034322E3022680A5441514141414145414143635141414141454B47486762593433505330794F6F4F33675531394D4C317A314D6E2B4D47397050376F6B3357625774796167716B4F75427556436E4561726A757875624B3937513D3D121087EE89A0B3464A4288688584B9607881");
            //Assert.Equal(data, data2);
            //var value = method.RequestMarshaller.Deserializer(data2);

            //{
            //    var method = DomainGrpcMethod<DomainGrpcRequest<DomainGrpcArgument<Guid, string>>, DomainGrpcResponse>.CreateMethod("test", "test");
            //    DomainGrpcResponse response = new DomainGrpcResponse
            //    {
            //        Exception = new DomainGrpcException(new ArgumentException("Test")),
            //        OS = "UnitTest"
            //    };
            //    var data = method.ResponseMarshaller.Serializer(response);
            //}
            //{
            //    var method = DomainGrpcMethod<DomainGrpcRequest, DomainGrpcResponse<string>>.CreateMethod("test", "test");
            //    DomainGrpcResponse<string> response = new DomainGrpcResponse<string>
            //    {
            //        OS = "UnitTest"
            //    };
            //    var data = method.ResponseMarshaller.Serializer(response);
            //}

            {
                DomainGrpcResponse<string> model = new DomainGrpcResponse<string>
                {
                    OS = "Test OS",
                    Result = "Test"
                };
                var data = Message.SerializeToBytes(model);
                var model2 = Message.DeserializeFromBytes<DomainGrpcResponse<string>>(data);
                Assert.Equal(model.OS, model.OS);
                Assert.Equal(model.Result, model.Result);
            }

            {
                DomainGrpcResponse<List<string>> model = new DomainGrpcResponse<List<string>>
                {
                    OS = "Test OS",
                    Result = new List<string> { "A", "B", "C" }
                };
                var data = Message.SerializeToBytes(model);
                var model2 = Message.DeserializeFromBytes<DomainGrpcResponse<List<string>>>(data);
                Assert.Equal(model.OS, model.OS);
                Assert.Equal(model.Result, model.Result);
            }

            {
                DomainGrpcResponse<string[]> model = new DomainGrpcResponse<string[]>
                {
                    OS = "Test OS",
                    Result = new string[] { "A", "B", "C" }
                };
                var data = Message.SerializeToBytes(model);
                var model2 = Message.DeserializeFromBytes<DomainGrpcResponse<string[]>>(data);
                Assert.Equal(model.OS, model.OS);
                Assert.Equal(model.Result, model.Result);
            }

            //var generator = new Lokad.ILPack.AssemblyGenerator();
            //// for ad-hoc serialization
            //var bytes = generator.GenerateAssemblyBytes(DomainGrpcTypeBuilder.GetAssembly());
            //File.WriteAllBytes("dynamic.dll", bytes);

        }
    }
}
