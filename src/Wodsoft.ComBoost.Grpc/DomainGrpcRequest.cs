using Google.Protobuf;
using Google.Protobuf.Collections;
using Google.Protobuf.Reflection;
using System;
using System.Collections.Generic;
using System.Text;
using Wodsoft.Protobuf;

namespace Wodsoft.ComBoost.Grpc
{
    public class DomainGrpcRequest : Message, IDomainRpcRequest
    {
        public DomainGrpcRequest() { }

        public DomainGrpcRequest(DomainGrpcRequest request)
        {

        }

        public string OS { get; set; }

        private static readonly MapField<string, string>.Codec _HeaderCodec = new MapField<string, string>.Codec(FieldCodec.ForString(10), FieldCodec.ForString(18), 18);
        private MapField<string, string> _headers { get; set; } = new MapField<string, string>();
        public IDictionary<string, string> Headers { get => _headers; }

        private static readonly MapField<string, string>.Codec _ValuesCodec = new MapField<string, string>.Codec(FieldCodec.ForString(10), FieldCodec.ForString(18), 26);
        private MapField<string, string> _values { get; set; } = new MapField<string, string>();
        public IDictionary<string, string> Values { get => _values; }

        protected override int CalculateSize()
        {
            int size = 0;
            if (OS != null)
                size += 1 + CodedOutputStream.ComputeStringSize(OS);
            size += _headers.CalculateSize(_HeaderCodec);
            size += _values.CalculateSize(_ValuesCodec);
            return size;
        }

        protected override void Write(ref WriteContext writer)
        {
            if (OS != null)
            {
                writer.WriteRawTag(10);
                writer.WriteString(OS);
            }
            _headers.WriteTo(ref writer, _HeaderCodec);
            _values.WriteTo(ref writer, _ValuesCodec);
        }

        protected override void Read(ref ParseContext parser)
        {
            uint tag;
            while ((tag = parser.ReadTag()) != 0)
            {
                switch (tag)
                {
                    case 10:
                        OS = parser.ReadString();
                        break;
                    case 18:
                        _headers.AddEntriesFrom(ref parser, _HeaderCodec);
                        break;
                    case 26:
                        _values.AddEntriesFrom(ref parser, _ValuesCodec);
                        break;
                    default:
                        ReadTag(tag, ref parser);
                        break;
                }
            }
        }

        protected virtual void ReadTag(uint tag, ref ParseContext parser) { }
    }

    public class DomainGrpcRequest<T> : DomainGrpcRequest
    {
        public T Argument { get; set; }

        protected override int CalculateSize()
        {
            Message<T> message = Argument;
            var size = base.CalculateSize();
            size += 1 + CodedOutputStream.ComputeMessageSize(message);
            return size;
        }

        protected override void Write(ref WriteContext writer)
        {
            base.Write(ref writer);
            if (Argument != null)
            {
                Message<T> message = Argument;
                writer.WriteRawTag(34);
                writer.WriteMessage(message);
            }
        }

        protected override void ReadTag(uint tag, ref ParseContext parser)
        {
            if (tag == 34)
            {
                Argument = default(T);
                Message<T> message = Argument;
                parser.ReadMessage(message);
            }
        }
    }
}
