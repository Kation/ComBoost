using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.AspNetCore
{
    public class JsonSerializerProvider : ISerializerProvider
    {
        public JsonSerializerProvider() : this(new JsonSerializerSettings()) { }

        public JsonSerializerProvider(JsonSerializerSettings settings)
        {
            Settings = settings;
        }

        public JsonSerializerSettings Settings { get; private set; }

        public ISerializer GetSerializer(Type type)
        {
            return new JsonSerializer(type, Settings);
        }
    }

    public class JsonSerializer : ISerializer
    {
        public JsonSerializer(Type type, JsonSerializerSettings settings)
        {
            Type = type;
            Settings = settings;
        }

        public Type Type { get; private set; }

        public JsonSerializerSettings Settings { get; private set; }

        public object Deserialize(Stream stream)
        {
            var reader = new StreamReader(stream);
            var value = reader.ReadToEnd();
            return JsonConvert.DeserializeObject(value, Type);
        }

        public void Serialize(Stream stream, object value)
        {
            var text = JsonConvert.SerializeObject(value, Type, Settings);
            var writer = new StreamWriter(stream);
            writer.Write(text);
            writer.Close();
        }
    }
}
