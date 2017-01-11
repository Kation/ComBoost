using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity;
using Wodsoft.ComBoost.Data.Entity.Metadata;
using Wodsoft.ComBoost.Security;

namespace Wodsoft.ComBoost.Mvc
{
    public class EntityJsonConverter : JsonConverter
    {
        public EntityJsonConverter(EntityAuthorizeAction action, ClaimsPrincipal principal)
        {
            Principal = principal;
            Action = action;
        }

        public EntityAuthorizeAction Action { get; private set; }

        public ClaimsPrincipal Principal { get; private set; }

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
            IEnumerable<IPropertyMetadata> propertyMetadatas;
            switch (Action)
            {
                case EntityAuthorizeAction.View:
                    propertyMetadatas = metadata.ViewProperties.Where(t =>
                    {
                        if (!t.AllowAnonymous && !Principal.Identity.IsAuthenticated)
                            return false;
                        if (t.AuthenticationRequiredMode == AuthenticationRequiredMode.All)
                            return t.ViewRoles.All(r => Principal.IsInDynamicRole(r));
                        else
                            return t.ViewRoles.Any(r => Principal.IsInDynamicRole(r));
                    }).ToArray();
                    break;
                case EntityAuthorizeAction.Create:
                    propertyMetadatas = metadata.CreateProperties.Where(t =>
                    {
                        if (!t.AllowAnonymous && !Principal.Identity.IsAuthenticated)
                            return false;
                        if (t.AuthenticationRequiredMode == AuthenticationRequiredMode.All)
                            return t.AddRoles.All(r => Principal.IsInDynamicRole(r));
                        else
                            return t.AddRoles.Any(r => Principal.IsInDynamicRole(r));
                    }).ToArray();
                    break;
                case EntityAuthorizeAction.Edit:
                    propertyMetadatas = metadata.EditProperties.Where(t =>
                    {
                        if (!t.AllowAnonymous && !Principal.Identity.IsAuthenticated)
                            return false;
                        if (t.AuthenticationRequiredMode == AuthenticationRequiredMode.All)
                            return t.EditRoles.All(r => Principal.IsInDynamicRole(r));
                        else
                            return t.EditRoles.Any(r => Principal.IsInDynamicRole(r));
                    }).ToArray();
                    break;
                default:
                    throw new NotSupportedException();
            }

            writer.WriteStartObject();
            writer.WritePropertyName(metadata.KeyProperty.ClrName);
            writer.WriteValue(metadata.KeyProperty.GetValue(entity));
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
