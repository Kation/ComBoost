using Google.Protobuf;
using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.Text;
using Wodsoft.Protobuf;

namespace Wodsoft.ComBoost.Grpc
{
    public class DomainGrpcTrace : Message, IDomainRpcTrace, IMessage<DomainGrpcTrace>
    {
        public DateTimeOffset StartTime { get; set; }

        public DateTimeOffset EndTime { get; set; }

        public TimeSpan ElapsedTime { get; set; }

        private static readonly FieldCodec<DomainGrpcTrace> _InnerCallCodec = FieldCodec.ForMessage(26, new MessageParser<DomainGrpcTrace>(() => new DomainGrpcTrace()));
        private RepeatedField<DomainGrpcTrace> _innerCall = new RepeatedField<DomainGrpcTrace>();
        public ICollection<DomainGrpcTrace> InnerCall => _innerCall;
        IEnumerable<IDomainRpcTrace> IDomainRpcTrace.InnerCall => _innerCall;

        protected override int CalculateSize()
        {
            int size = 0;
            size += 1 + CodedOutputStream.ComputeMessageSize(Timestamp.FromDateTimeOffset(StartTime));
            size += 1 + CodedOutputStream.ComputeMessageSize(Timestamp.FromDateTimeOffset(EndTime));
            size += 1 + CodedOutputStream.ComputeMessageSize(Duration.FromTimeSpan(ElapsedTime));
            size += _innerCall.CalculateSize(_InnerCallCodec);
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
                        var startTime = new Timestamp();
                        parser.ReadMessage(startTime);
                        StartTime = startTime.ToDateTimeOffset();
                        break;
                    case 18:
                        var endTime = new Timestamp();
                        parser.ReadMessage(endTime);
                        EndTime = endTime.ToDateTimeOffset();
                        break;
                    case 26:
                        var elapsedTime = new Duration();
                        parser.ReadMessage(elapsedTime);
                        ElapsedTime = elapsedTime.ToTimeSpan();
                        break;
                    case 34:
                        _innerCall.AddEntriesFrom(ref parser, _InnerCallCodec);
                        break;
                }
            }
        }

        protected override void Write(ref WriteContext writer)
        {
            writer.WriteRawTag(10);
            writer.WriteMessage(Timestamp.FromDateTimeOffset(StartTime));
            writer.WriteRawTag(18);
            writer.WriteMessage(Timestamp.FromDateTimeOffset(EndTime));
            writer.WriteRawTag(26);
            writer.WriteMessage(Duration.FromTimeSpan(ElapsedTime));
            _innerCall.WriteTo(ref writer, _InnerCallCodec);
        }

        void IMessage<DomainGrpcTrace>.MergeFrom(DomainGrpcTrace message)
        {
            throw new NotSupportedException();
        }

        bool IEquatable<DomainGrpcTrace>.Equals(DomainGrpcTrace other)
        {
            throw new NotSupportedException();
        }

        DomainGrpcTrace IDeepCloneable<DomainGrpcTrace>.Clone()
        {
            throw new NotSupportedException();
        }
    }
}
