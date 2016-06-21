using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Security
{
    public abstract class GeneralSecurityProvider : ISecurityProvider
    {
        public string ConvertRoleToString(object role)
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role));
            if (role is IRoleName)
                return ((IRoleName)role).RoleName;
            Type type = role.GetType();
            if (type.GetTypeInfo().IsValueType)
            {
                string name = type.FullName + "-";
                name += role.ToString();
                return name;
            }
            throw new NotSupportedException("不支持将该对象转换为字符串。");
        }

        public Task<IPermission> GetPermissionAsync(string identity)
        {
            return GetPermissionByIdentity(identity);
        }

        public Task<IPermission> GetPermissionAsync(IDictionary<string, string> properties)
        {
            if (properties.ContainsKey("username"))
                return GetPermissionByUsername(properties["username"]);
            else if (properties.ContainsKey("identity"))
                return GetPermissionByIdentity(properties["identity"]);
            else if (properties.ContainsKey("id"))
                return GetPermissionByIdentity(properties["id"]);
            else
                throw new NotSupportedException("不支持的属性。");
        }

        protected abstract Task<IPermission> GetPermissionByIdentity(string identity);

        protected abstract Task<IPermission> GetPermissionByUsername(string username);
    }
}
