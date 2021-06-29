using Google.Protobuf;
using Google.Protobuf.Collections;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Wodsoft.Protobuf;
using Wodsoft.Protobuf.Generators;

namespace Wodsoft.ComBoost.Grpc
{
    public class DomainGrpcResponse : Message, IDomainRpcResponse
    {
        public string OS { get; set; }

        private static readonly MapField<string, byte[]>.Codec _HeaderCodec = new MapField<string, byte[]>.Codec(FieldCodec.ForString(10), new ByteArrayCodeGenerator().CreateFieldCodec(2), 18);
        private MapField<string, byte[]> _headers { get; set; } = new MapField<string, byte[]>();
        public IDictionary<string, byte[]> Headers { get => _headers; }

        public DomainGrpcTrace Trace { get; set; }
        IDomainRpcTrace IDomainRpcResponse.Trace => Trace;

        public DomainGrpcException Exception { get; set; }
        IDomainRpcException IDomainRpcResponse.Exception => Exception;

        protected override int CalculateSize()
        {
            int size = 0;
            if (OS != null)
                size += 1 + CodedOutputStream.ComputeStringSize(OS);
            size += _headers.CalculateSize(_HeaderCodec);
            if (Trace != null)
                size += 1 + CodedOutputStream.ComputeMessageSize(Trace);
            if (Exception != null)
                size += 1 + CodedOutputStream.ComputeMessageSize(Exception);
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
                        OS = parser.ReadString();
                        break;
                    case 18:
                        _headers.AddEntriesFrom(ref parser, _HeaderCodec);
                        break;
                    case 26:
                        Trace = new DomainGrpcTrace();
                        parser.ReadMessage(Trace);
                        break;
                    case 34:
                        Exception = new DomainGrpcException();
                        parser.ReadMessage(Exception);
                        break;
                    default:
                        ReadTag(tag, ref parser);
                        break;
                }
            }
        }

        protected virtual void ReadTag(uint tag, ref ParseContext parser) { }

        protected override void Write(ref WriteContext writer)
        {
            if (OS != null)
            {
                writer.WriteRawTag(10);
                writer.WriteString(OS);
            }
            _headers.WriteTo(ref writer, _HeaderCodec);
            if (Trace != null)
            {
                writer.WriteRawTag(26);
                writer.WriteMessage(Trace);
            }
            if (Exception != null)
            {
                writer.WriteRawTag(34);
                writer.WriteMessage(Exception);
            }
        }
    }

    public class DomainGrpcResponse<T> : DomainGrpcResponse, IDomainRpcResponse<T>
    {
        private T _result;
        public T Result { get => _result; set => _result = value; }

        protected override int CalculateSize()
        {
            var size = base.CalculateSize();
            size += _CalculateResultSize(this, ref _result);
            return size;
        }

        protected override void ReadTag(uint tag, ref ParseContext parser)
        {
            if (tag == _ResultTag)
            {
                Result = _ReadResult(this, ref parser);
            }
        }

        protected override void Write(ref WriteContext writer)
        {
            base.Write(ref writer);
            _WriteResult(this, ref writer, _result);
        }

        private static readonly uint _ResultTag;
        private static readonly CalculateResultSize _CalculateResultSize;
        private static readonly ParseResult _ReadResult;
        private static readonly WriteResult _WriteResult;
        private delegate int CalculateResultSize(DomainGrpcResponse<T> source, ref T result);
        private delegate void WriteResult(DomainGrpcResponse<T> source, ref WriteContext writer, T result);
        private delegate T ParseResult(DomainGrpcResponse<T> source, ref ParseContext parser);

        static DomainGrpcResponse()
        {
            var resultType = typeof(T);
            var codeGenerator = MessageBuilder.GetCodeGenerator<T>();

            DynamicMethod calculateSize = new DynamicMethod("CalculateResultSize", typeof(int), new Type[] { typeof(DomainGrpcResponse<T>), resultType.MakeByRefType() });
            var calculateSizeILGenerator = calculateSize.GetILGenerator();
            var calculateSizeEnd = calculateSizeILGenerator.DefineLabel();

            DynamicMethod write = new DynamicMethod("WriteResult", null, new Type[] { typeof(DomainGrpcResponse<T>), typeof(WriteContext).MakeByRefType(), resultType });
            var writeILGenerator = write.GetILGenerator();
            var writeEnd = writeILGenerator.DefineLabel();

            DynamicMethod read = new DynamicMethod("ReadResult", resultType, new Type[] { typeof(DomainGrpcResponse<T>), typeof(ParseContext).MakeByRefType() });
            var readILGenerator = read.GetILGenerator();

            //CalculateSize
            var calculateSizeValueVariable = calculateSizeILGenerator.DeclareLocal(resultType.MakeByRefType());
            calculateSizeILGenerator.Emit(OpCodes.Ldarg_1);
            calculateSizeILGenerator.Emit(OpCodes.Stloc, calculateSizeValueVariable);
            //Write
            var writeValueVariable = writeILGenerator.DeclareLocal(resultType.MakeByRefType());
            writeILGenerator.Emit(OpCodes.Ldarg_2);
            writeILGenerator.Emit(OpCodes.Stloc, writeValueVariable);


            if (codeGenerator == null)
            {
                if (!resultType.IsClass)
                    throw new NotSupportedException($"Do not support response type \"{resultType.FullName}\".");
                if (resultType.GetConstructor(Array.Empty<Type>()) == null)
                    throw new NotSupportedException($"Response type \"{resultType.FullName}\" doesn't have a default constructor.");
                codeGenerator = (ICodeGenerator<T>)Activator.CreateInstance(typeof(ObjectCodeGenerator<>).MakeGenericType(resultType));
            }
            _ResultTag = WireFormat.MakeTag(5, codeGenerator.WireType);
            //CalculateSize
            {
                codeGenerator.GenerateCalculateSizeCode(calculateSizeILGenerator, calculateSizeValueVariable);
            }
            //Read
            {
                codeGenerator.GenerateReadCode(readILGenerator);
            }
            //Write
            {
                codeGenerator.GenerateWriteCode(writeILGenerator, writeValueVariable, 5);
            }
            calculateSizeILGenerator.MarkLabel(calculateSizeEnd);
            calculateSizeILGenerator.Emit(OpCodes.Ret);
            readILGenerator.Emit(OpCodes.Ret);
            writeILGenerator.MarkLabel(writeEnd);
            writeILGenerator.Emit(OpCodes.Ret);

            _CalculateResultSize = (CalculateResultSize)calculateSize.CreateDelegate(typeof(CalculateResultSize));
            _ReadResult = (ParseResult)read.CreateDelegate(typeof(ParseResult));
            _WriteResult = (WriteResult)write.CreateDelegate(typeof(WriteResult));
        }
    }

    public class DomainGrpcCollectionResponse<TContainer, TElement> : DomainGrpcResponse, IDomainRpcResponse<TContainer>
        where TContainer : class, ICollection<TElement>
        where TElement : new()
    {
        private TContainer _result;
        public TContainer Result { get => _result; set => _result = value; }

        private RepeatedField<TElement> _container;

        protected override int CalculateSize()
        {
            var size = base.CalculateSize();
            var container = new RepeatedField<TElement>();
            container.AddRange(Result);
            size += container.CalculateSize(_ResultCodec);
            return size;
        }

        protected override void Read(ref ParseContext parser)
        {
            _container = new RepeatedField<TElement>();
            base.Read(ref parser);
        }

        protected override void ReadTag(uint tag, ref ParseContext parser)
        {
            if (tag == _ResultTag1 || tag == _ResultTag2)
            {
                _container.AddEntriesFrom(ref parser, _ResultCodec);
            }
        }

        protected override void Write(ref WriteContext writer)
        {
            base.Write(ref writer);
            var container = new RepeatedField<TElement>();
            container.AddRange(Result);
            container.WriteTo(ref writer, _ResultCodec);
        }

        private static readonly uint _ResultTag1, _ResultTag2;
        private static FieldCodec<TElement> _ResultCodec;
        static DomainGrpcCollectionResponse()
        {
            _ResultTag1 = WireFormat.MakeTag(4, WireFormat.WireType.LengthDelimited);
            var codeGenerator = MessageBuilder.GetCodeGenerator<TElement>();
            if (codeGenerator == null)
                codeGenerator = new ObjectCodeGenerator<TElement>();
            if (codeGenerator.WireType == WireFormat.WireType.Varint)
                _ResultTag2 = WireFormat.MakeTag(4, WireFormat.WireType.Varint);
            _ResultCodec = codeGenerator.CreateFieldCodec(4);
        }
    }

    public class DomainGrpcDictionaryResponse<TContainer, TKey, TValue> : DomainGrpcResponse, IDomainRpcResponse<TContainer>
        where TContainer : class, IDictionary<TKey, TValue>
        where TKey : new()
        where TValue : new()
    {
        private TContainer _result;
        public TContainer Result { get => _result; set => _result = value; }

        private MapField<TKey, TValue> _container;

        protected override int CalculateSize()
        {
            var size = base.CalculateSize();
            var container = new MapField<TKey, TValue>();
            container.Add(Result);
            size += container.CalculateSize(_ResultCodec);
            return size;
        }

        protected override void Read(ref ParseContext parser)
        {
            _container = new MapField<TKey, TValue>();
            base.Read(ref parser);
        }

        protected override void ReadTag(uint tag, ref ParseContext parser)
        {
            if (tag == _ResultTag)
            {
                _container.AddEntriesFrom(ref parser, _ResultCodec);
            }
        }

        private static readonly uint _ResultTag;
        private static MapField<TKey, TValue>.Codec _ResultCodec;
        static DomainGrpcDictionaryResponse()
        {
            _ResultTag = WireFormat.MakeTag(4, WireFormat.WireType.LengthDelimited);
            FieldCodec<TKey> keyCodec;
            FieldCodec<TValue> valueCodec;
            {
                var codeGenerator = MessageBuilder.GetCodeGenerator<TKey>();
                if (codeGenerator == null)
                    codeGenerator = new ObjectCodeGenerator<TKey>();
                keyCodec = codeGenerator.CreateFieldCodec(1);
            }
            {
                var codeGenerator = MessageBuilder.GetCodeGenerator<TValue>();
                if (codeGenerator == null)
                    codeGenerator = new ObjectCodeGenerator<TValue>();
                valueCodec = codeGenerator.CreateFieldCodec(1);
            }
            _ResultCodec = new MapField<TKey, TValue>.Codec(keyCodec, valueCodec, WireFormat.MakeTag(4, WireFormat.WireType.LengthDelimited));
        }
    }
}
