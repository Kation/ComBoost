using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity.Metadata;

namespace Wodsoft.ComBoost.Mvc
{
    public class PropertyMetadataJsonConverter : JsonConverter
    {
        public static readonly PropertyMetadataJsonConverter Converter = new PropertyMetadataJsonConverter();

        public override bool CanConvert(Type objectType)
        {
            return typeof(IPropertyMetadata).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            IPropertyMetadata metadata = (IPropertyMetadata)value;

            writer.WriteStartObject();

            writer.WritePropertyName("Name");
            writer.WriteValue(metadata.Name);
            writer.WritePropertyName("ClrName");
            writer.WriteValue(metadata.ClrName);
            writer.WritePropertyName("ShortName");
            writer.WriteValue(metadata.ShortName);
            writer.WritePropertyName("Description");
            writer.WriteValue(metadata.Description);
            writer.WritePropertyName("Order");
            writer.WriteValue(metadata.Order);
            writer.WritePropertyName("Searchable");
            writer.WriteValue(metadata.Searchable);
            writer.WritePropertyName("IsRequired");
            writer.WriteValue(metadata.IsRequired);
            writer.WritePropertyName("Type");
            writer.WriteValue(metadata.Type);
            writer.WritePropertyName("CustomType");
            writer.WriteValue(metadata.CustomType);

            writer.WriteEndObject();
        }
    }
}
