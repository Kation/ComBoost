using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Mvc
{
    public class EntityEditModelJsonConverter : JsonConverter<IEntityEditModel>
    {
        public static readonly EntityEditModelJsonConverter Converter = new EntityEditModelJsonConverter();

        public override bool CanConvert(Type objectType)
        {
            return typeof(IEntityEditModel).IsAssignableFrom(objectType);
        }

        public override IEntityEditModel Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotSupportedException();
        }

        public override void Write(Utf8JsonWriter writer, IEntityEditModel value, JsonSerializerOptions options)
        {
            IEntityEditModel model = value;
            writer.WriteStartObject();

            writer.WritePropertyName("Item");
            JsonSerializer.Serialize(writer, model.Item, options);

            writer.WritePropertyName("Properties");
            JsonSerializer.Serialize(writer, model.Properties, options);

            writer.WritePropertyName("Metadata");
            JsonSerializer.Serialize(writer, model.Metadata, options);

            writer.WriteEndObject();
        }
    }
}
