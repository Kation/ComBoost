using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Mvc
{
    public class EntityEditModelJsonConverter : JsonConverter
    {
        public static readonly EntityEditModelJsonConverter Converter = new EntityEditModelJsonConverter();

        public override bool CanConvert(Type objectType)
        {
            return typeof(IEntityEditModel).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotSupportedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            IEntityEditModel model = (IEntityEditModel)value;
            writer.WriteStartObject();
            
            writer.WritePropertyName("Item");
            serializer.Serialize(writer, model.Item);

            writer.WritePropertyName("Properties");
            serializer.Serialize(writer, model.Properties);

            writer.WritePropertyName("Metadata");
            serializer.Serialize(writer, model.Metadata);

            writer.WriteEndObject();
        }
    }
}
