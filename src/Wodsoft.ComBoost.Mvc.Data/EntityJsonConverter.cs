using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data;
using Wodsoft.ComBoost.Data.Entity;
using Wodsoft.ComBoost.Data.Entity.Metadata;
using Wodsoft.ComBoost.Security;

namespace Wodsoft.ComBoost.Mvc
{
    public class EntityJsonConverter : JsonConverter<IEntity>
    {
        public EntityJsonConverter(EntityDomainAuthorizeOption option, IAuthentication authentication)
        {
            if (option == null)
                throw new ArgumentNullException(nameof(option));
            if (authentication == null)
                throw new ArgumentNullException(nameof(authentication));
            Authentication = authentication;
            Option = option;
        }

        public EntityDomainAuthorizeOption Option { get; private set; }

        public IAuthentication Authentication { get; private set; }

        public override bool CanConvert(Type objectType)
        {
            return typeof(IEntity).IsAssignableFrom(objectType);
        }

        public override IEntity Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotSupportedException();
        }

        public override void Write(Utf8JsonWriter writer, IEntity value, JsonSerializerOptions options)
        {
            IEntity entity = value;

            var metadata = EntityDescriptor.GetMetadata(value.GetType());
            IPropertyMetadata[] propertyMetadatas = Option.GetProperties(metadata, Authentication).ToArray();

            writer.WriteStartObject();
            if (!propertyMetadatas.Contains(metadata.KeyProperty))
            {
                writer.WritePropertyName(metadata.KeyProperty.ClrName);
                JsonSerializer.Serialize(writer, metadata.KeyProperty.GetValue(entity), options);
            }
            foreach (var property in propertyMetadatas)
            {
                object propertyValue = property.GetValue(value);
                if (propertyValue == null)
                    continue;
                writer.WritePropertyName(property.ClrName);
                if (property.Type == CustomDataType.Other)
                {
                    if (property.CustomType == "Entity")
                    {
                        var item = (IEntity)propertyValue;
                        writer.WriteStartObject();
                        writer.WriteString("Index", item.Index.ToString());
                        writer.WriteString("Name", item.ToString());
                        writer.WriteEndObject();
                    }
                    else if (property.CustomType == "Enum")
                        JsonSerializer.Serialize(writer, property.GetValue(entity), options);
                    else if (property.CustomType == "Collection")
                        continue;
                    else
                        throw new NotSupportedException("不支持序列化的属性“" + property.Name + "”");
                }
                else
                    JsonSerializer.Serialize(writer, propertyValue, options);
            }

            writer.WritePropertyName("IsEditAllowed");
            writer.WriteBooleanValue(entity.IsEditAllowed);
            writer.WritePropertyName("IsRemoveAllowed");
            writer.WriteBooleanValue(entity.IsRemoveAllowed);

            writer.WriteEndObject();
        }
    }
}
