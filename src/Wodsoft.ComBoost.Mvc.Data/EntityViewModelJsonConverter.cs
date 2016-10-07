using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Mvc
{
    public class EntityViewModelJsonConverter : JsonConverter
    {
        public static readonly EntityViewModelJsonConverter Converter = new EntityViewModelJsonConverter();

        public override bool CanConvert(Type objectType)
        {
            return typeof(IEntityViewModel).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotSupportedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            IEntityViewModel model = (IEntityViewModel)value;
            writer.WriteStartObject();

            writer.WritePropertyName("PageSizeOption");
            serializer.Serialize(writer, model.PageSizeOption);
            writer.WritePropertyName("TotalPage");
            writer.WriteValue(model.TotalPage);
            writer.WritePropertyName("CurrentPage");
            writer.WriteValue(model.CurrentPage);
            writer.WritePropertyName("CurrentSize");
            writer.WriteValue(model.CurrentSize);

            writer.WritePropertyName("ViewButtons");
            serializer.Serialize(writer, model.ViewButtons);
            writer.WritePropertyName("ItemButtons");
            serializer.Serialize(writer, model.ItemButtons);

            writer.WritePropertyName("Items");
            serializer.Serialize(writer, model.Properties);

            writer.WritePropertyName("Properties");
            serializer.Serialize(writer, model.Properties);

            writer.WritePropertyName("SearchItem");
            serializer.Serialize(writer, model.SearchItem);

            writer.WriteEndObject();
        }
    }
}
