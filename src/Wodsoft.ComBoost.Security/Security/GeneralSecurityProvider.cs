using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Security
{
    /// <summary>
    /// 通用安全提供器。
    /// </summary>
    public abstract class GeneralSecurityProvider : ISecurityProvider
    {
        /// <inheritdoc/>
        public virtual string ConvertRoleToString(object role)
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role));
            if (role is string)
                return (string)role;
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

        /// <inheritdoc/>
        public virtual Task<IPermission> GetPermissionAsync(string identity)
        {
            return GetPermissionByIdentity(identity);
        }

        /// <inheritdoc/>
        public virtual Task<IPermission> GetPermissionAsync(IDictionary<string, string> properties)
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

        /// <summary>
        /// 根据用户Id获取权限对象。
        /// </summary>
        /// <param name="identity">用户Id。</param>
        /// <returns></returns>
        protected abstract Task<IPermission> GetPermissionByIdentity(string identity);

        /// <summary>
        /// 根据用户名获取权限对象。
        /// </summary>
        /// <param name="username">用户名。</param>
        /// <returns></returns>
        protected abstract Task<IPermission> GetPermissionByUsername(string username);
    }
}
