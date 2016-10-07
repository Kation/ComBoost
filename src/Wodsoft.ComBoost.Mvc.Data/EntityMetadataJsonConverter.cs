using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity.Metadata;

namespace Wodsoft.ComBoost.Mvc
{
    public class EntityMetadataJsonConverter : JsonConverter
    {
        public static readonly EntityMetadataJsonConverter Converter = new EntityMetadataJsonConverter();

        public override bool CanConvert(Type objectType)
        {
            return typeof(IEntityMetadata).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            IEntityMetadata metadata = (IEntityMetadata)value;

            writer.WriteStartObject();

            writer.WritePropertyName("Name");
            writer.WriteValue(metadata.Name);

            writer.WritePropertyName("Key");
            writer.WriteValue(metadata.KeyProperty.ClrName);

            writer.WriteEndObject();
        }
    }
}
