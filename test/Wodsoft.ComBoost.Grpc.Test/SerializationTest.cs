//using Google.Protobuf;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Threading.Tasks;
//using Xunit;

//namespace Wodsoft.ComBoost.Grpc.Test
//{
//    public class SerializationTest
//    {
//        [Fact]
//        public void Simply_Class_Test()
//        {
//            var type = DomainGrpcTypeBuilder.GetGrpcType<SimplyObject>();

//            //var generator = new Lokad.ILPack.AssemblyGenerator();
//            //// for ad-hoc serialization
//            //var bytes = generator.GenerateAssemblyBytes(DomainGrpcTypeBuilder.GetAssembly());
//            //File.WriteAllBytes("dynamic.dll", bytes);

//            SimplyObject obj = new SimplyObject
//            {
//                IntValue = 100,
//                DoubleValue = 222.222,
//                StringValue = "This is a simply object"
//            };

//            DomainGrpcType<SimplyObject> grpcObject = (DomainGrpcType<SimplyObject>)Activator.CreateInstance(type);
//            IMessage message = grpcObject;
//            var size = message.CalculateSize();

//            byte[] data = new byte[message.CalculateSize()];
//            using (CodedOutputStream cos = new CodedOutputStream(data))
//            {
//                message.WriteTo(cos);
//            }
//        }
//    }
//}
