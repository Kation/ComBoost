using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    public class StringSeralizerProvider : ISerializerProvider
    {
        private ConcurrentDictionary<Type, ISerializer> _Seralizers;

        public StringSeralizerProvider()
        {
            _Seralizers = new ConcurrentDictionary<Type, ISerializer>();
        }

        public ISerializer GetSerializer(Type type)
        {
            return _Seralizers.GetOrAdd(type, t =>
            {
                var converter = TypeDescriptor.GetConverter(type);
                if (converter == null)
                    throw new NotSupportedException("不支持的类型。");
                if (!converter.CanConvertTo(typeof(string)))
                    throw new NotSupportedException("不支持的类型。");
                if (!converter.CanConvertFrom(typeof(string)))
                    throw new NotSupportedException("不支持的类型。");
                return new StringSeralizer(converter);
            });
        }
    }

    public class StringSeralizer : ISerializer
    {
        public StringSeralizer(TypeConverter converter)
        {
            if (converter == null)
                throw new ArgumentNullException(nameof(converter));
            Converter = converter;
        }

        public TypeConverter Converter { get; private set; }

        public object Deserialize(Stream stream)
        {
            StreamReader reader = new StreamReader(stream, Encoding.UTF8);
            return Converter.ConvertFromString(reader.ReadToEnd());
        }

        public void Serialize(Stream stream, object value)
        {
            var data = Encoding.UTF8.GetBytes(Converter.ConvertToString(value));
            stream.Write(data, 0, data.Length);
        }
    }
}
