using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity.Metadata;

namespace Wodsoft.ComBoost.Mvc
{
    public class EntityMetadataJsonConverter : JsonConverter<IEntityMetadata>
    {
        public static readonly EntityMetadataJsonConverter Converter = new EntityMetadataJsonConverter();

        public override bool CanConvert(Type objectType)
        {
            return typeof(IEntityMetadata).IsAssignableFrom(objectType);
        }

        public override IEntityMetadata Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotSupportedException();
        }

        public override void Write(Utf8JsonWriter writer, IEntityMetadata value, JsonSerializerOptions options)
        {
            IEntityMetadata metadata = (IEntityMetadata)value;

            writer.WriteStartObject();

            writer.WritePropertyName("Name");
            writer.WriteStringValue(metadata.Name);

            writer.WritePropertyName("Key");
            writer.WriteStringValue(metadata.KeyProperty.ClrName);

            writer.WriteEndObject();
        }
    }
}
