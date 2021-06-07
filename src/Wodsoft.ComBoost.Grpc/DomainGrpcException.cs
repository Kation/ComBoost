using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Text;
using Wodsoft.Protobuf;

namespace Wodsoft.ComBoost.Grpc
{
    public class DomainGrpcException : Message, IDomainRpcException
    {
        public DomainGrpcException() { }

        public DomainGrpcException(Exception ex)
        {
            Title = ex.GetType().FullName;
            Message = ex.Message;
            Source = ex.Source;
            StackTrace = ex.StackTrace;
            if (ex.InnerException != null)
                InnerException = new DomainGrpcException(ex.InnerException);
        }

        public string Title { get; set; }

        public string Message { get; set; }

        public string Source { get; set; }

        public string StackTrace { get; set; }

        public DomainGrpcException InnerException { get; set; }
        IDomainRpcException IDomainRpcException.InnerException => InnerException;

        protected override int CalculateSize()
        {
            int size = 0;
            if (Title.Length != 0)
            {
                size += 1 + CodedOutputStream.ComputeStringSize(Title);
            }
            if (Message.Length != 0)
            {
                size += 1 + CodedOutputStream.ComputeStringSize(Message);
            }
            if (Source.Length != 0)
            {
                size += 1 + CodedOutputStream.ComputeStringSize(Source);
            }
            if (StackTrace.Length != 0)
            {
                size += 1 + CodedOutputStream.ComputeStringSize(StackTrace);
            }
            if (InnerException != null)
            {
                size += 1 + CodedOutputStream.ComputeMessageSize(InnerException);
            }
            return size;
        }

        protected override void Read(ref ParseContext parser)
        {
            uint tag;
            while ((tag = parser.ReadTag()) != 0)
            {
                switch (tag)
                {
                    case 10:
                        Title = parser.ReadString();
                        break;
                    case 18:
                        Message = parser.ReadString();
                        break;
                    case 26:
                        Source = parser.ReadString();
                        break;
                    case 34:
                        StackTrace = parser.ReadString();
                        break;
                    case 42:
                        InnerException = new DomainGrpcException();
                        parser.ReadMessage(InnerException);
                        break;
                }
            }
        }

        protected override void Write(ref WriteContext writer)
        {
            if (Title.Length != 0)
            {
                writer.WriteRawTag(10);
                writer.WriteString(Title);
            }
            if (Message.Length != 0)
            {
                writer.WriteRawTag(18);
                writer.WriteString(Message);
            }
            if (Source.Length != 0)
            {
                writer.WriteRawTag(26);
                writer.WriteString(Source);
            }
            if (StackTrace.Length != 0)
            {
                writer.WriteRawTag(34);
                writer.WriteString(StackTrace);
            }
            if (InnerException != null)
            {
                writer.WriteRawTag(42);
                writer.WriteMessage(InnerException);
            }
        }
    }
}
