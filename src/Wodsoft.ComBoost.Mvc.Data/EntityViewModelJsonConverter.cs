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
    public class EntityViewModelJsonConverter : JsonConverter<IEntityViewModel>
    {
        public static readonly EntityViewModelJsonConverter Converter = new EntityViewModelJsonConverter();

        public override bool CanConvert(Type objectType)
        {
            return typeof(IEntityViewModel).IsAssignableFrom(objectType);
        }

        public override IEntityViewModel Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotSupportedException();
        }

        public override void Write(Utf8JsonWriter writer, IEntityViewModel value, JsonSerializerOptions options)
        {
            IEntityViewModel model = value;
            writer.WriteStartObject();

            writer.WritePropertyName("PageSizeOption");
            JsonSerializer.Serialize(writer, model.PageSizeOption, options);
            writer.WritePropertyName("TotalPage");
            writer.WriteNumberValue(model.TotalPage);
            writer.WritePropertyName("CurrentPage");
            writer.WriteNumberValue(model.CurrentPage);
            writer.WritePropertyName("CurrentSize");
            writer.WriteNumberValue(model.CurrentSize);

            writer.WritePropertyName("ViewButtons");
            JsonSerializer.Serialize(writer, model.ViewButtons, options);
            writer.WritePropertyName("ItemButtons");
            JsonSerializer.Serialize(writer, model.ItemButtons, options);

            writer.WritePropertyName("Items");
            JsonSerializer.Serialize(writer, model.Items, options);

            writer.WritePropertyName("Properties");
            JsonSerializer.Serialize(writer, model.Properties, options);

            writer.WritePropertyName("SearchItem");
            JsonSerializer.Serialize(writer, model.SearchItem, options);

            writer.WritePropertyName("Metadata");
            JsonSerializer.Serialize(writer, model.Metadata, options);

            writer.WriteEndObject();
        }
    }
}
