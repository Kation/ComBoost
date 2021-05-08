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
    public class PropertyMetadataJsonConverter : JsonConverter<IPropertyMetadata>
    {
        public static readonly PropertyMetadataJsonConverter Converter = new PropertyMetadataJsonConverter();

        public override bool CanConvert(Type objectType)
        {
            return typeof(IPropertyMetadata).IsAssignableFrom(objectType);
        }

        public override IPropertyMetadata Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotSupportedException();
        }

        public override void Write(Utf8JsonWriter writer, IPropertyMetadata value, JsonSerializerOptions options)
        {
            IPropertyMetadata metadata = value;

            writer.WriteStartObject();

            writer.WritePropertyName("Name");
            writer.WriteStringValue(metadata.Name);
            writer.WritePropertyName("ClrName");
            writer.WriteStringValue(metadata.ClrName);
            writer.WritePropertyName("ShortName");
            writer.WriteStringValue(metadata.ShortName);
            writer.WritePropertyName("Description");
            writer.WriteStringValue(metadata.Description);
            writer.WritePropertyName("Order");
            writer.WriteNumberValue(metadata.Order);
            writer.WritePropertyName("Searchable");
            writer.WriteBooleanValue(metadata.Searchable);
            writer.WritePropertyName("IsRequired");
            writer.WriteBooleanValue(metadata.IsRequired);
            writer.WritePropertyName("Type");
            JsonSerializer.Serialize(writer, metadata.Type, options);
            writer.WritePropertyName("CustomType");
            writer.WriteStringValue(metadata.CustomType);

            writer.WriteEndObject();
        }
    }
}
