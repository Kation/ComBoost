using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data;
using Wodsoft.ComBoost.Data.Entity;
using Wodsoft.ComBoost.Data.Entity.Metadata;
using Wodsoft.ComBoost.Security;

namespace Wodsoft.ComBoost.Mvc
{
    public class EntityJsonConverter : JsonConverter
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

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotSupportedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            IEntity entity = (IEntity)value;
            var metadata = EntityDescriptor.GetMetadata(value.GetType());
            IPropertyMetadata[] propertyMetadatas = Option.GetProperties(metadata, Authentication).ToArray();

            writer.WriteStartObject();
            if (!propertyMetadatas.Contains(metadata.KeyProperty))
            {
                writer.WritePropertyName(metadata.KeyProperty.ClrName);
                writer.WriteValue(metadata.KeyProperty.GetValue(entity));
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
                        writer.WritePropertyName("Index");
                        writer.WriteValue(item.Index.ToString());
                        writer.WritePropertyName("Name");
                        writer.WriteValue(item.ToString());
                        writer.WriteEndObject();
                    }
                    else if (property.CustomType == "Enum")
                        writer.WriteValue(property.GetValue(entity));
                    else if (property.CustomType == "Collection")
                        continue;
                    else
                        throw new NotSupportedException("不支持序列化的属性“" + property.Name + "”");
                }
                else
                    writer.WriteValue(propertyValue);
            }

            writer.WritePropertyName("IsEditAllowed");
            writer.WriteValue(entity.IsEditAllowed);
            writer.WritePropertyName("IsRemoveAllowed");
            writer.WriteValue(entity.IsRemoveAllowed);

            writer.WriteEndObject();
        }
    }
}
