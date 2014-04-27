using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using Wodsoft.Net.Service;
using Wodsoft.Net.Communication;
using System.ComponentModel.DataAnnotations;

namespace System.Security
{
    public class SecurityEntityServiceFormatter : DataFormatter
    {
        private CacheEntityContextBuilder ContextBuilder;

        public SecurityEntityServiceFormatter(CacheEntityContextBuilder contextBuilder)
        {
            ContextBuilder = contextBuilder;
        }

        public override object Deserialize(Type objType, byte[] data)
        {
            if (data.Length == 16 && typeof(EntityBase).IsAssignableFrom(objType))
            {
                var queryable = ContextBuilder.GetContext(objType);
                return queryable.GetType().GetMethod("GetEntity").Invoke(queryable, new object[] { new Guid(data) });
            }
            return base.Deserialize(objType, data);
        }

        [ThreadStatic]
        private bool MainEntity;
        public override byte[] Serialize(object obj)
        {
            Type type = obj.GetType();
            if (!MainEntity && typeof(EntityBase).IsAssignableFrom(type))
            {
                return ((EntityBase)obj).Index.ToByteArray();
            }

            MainEntity = false;

            BinaryDataWriter writer = new BinaryDataWriter();

            var properties = type.GetProperties();
            for (byte i = 0; i < properties.Length; i++)
            {
                var property = properties[i];

                #region 判断是否有效数据

                if (!property.CanRead || !property.CanWrite)
                    continue;
                PropertyAuthenticationAttribute authenticate = property.GetCustomAttributes(typeof(PropertyAuthenticationAttribute), false).FirstOrDefault() as PropertyAuthenticationAttribute;
                if (authenticate != null)
                {
                    if (!authenticate.AllowAnonymous && !MemberManager.IsSigned)
                        continue;
                    if (!RoleManager.HasRoles(authenticate.ViewRolesRequired))
                        continue;
                }

                #endregion

                writer.WriteByte(i);

                var data = Serialize(property.GetValue(obj, null));
                writer.WriteBytes(data);
            }

            MainEntity = true;
            return writer.ToArray();
        }
    }
}
