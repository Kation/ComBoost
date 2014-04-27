using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace System.Security
{
    public static class RoleManager
    {
        private static RoleManagerProvider provider;

        static RoleManager()
        {
            Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (config.AppSettings.Settings.AllKeys.Contains("RoleManagerProvider"))
            {
                try
                {
                    Type type = System.Reflection.TypeDelegator.GetType(config.AppSettings.Settings["RoleManagerProvider"].Value);
                    if (type != null)
                    {
                        provider = (RoleManagerProvider)Activator.CreateInstance(type);
                        return;
                    }
                }
                catch
                {

                }
            }
            provider = new DefaultRoleManager();
        }

        public static bool IsEnabled { get { return provider != null; } }

        public static string[] GetRoles()
        {
            if (!IsEnabled)
                throw new InvalidOperationException("角色管理器没有开启。");
            return provider.GetRoles();
        }

        public static bool HasRoles(string[] roles)
        {
            if (!IsEnabled)
                throw new InvalidOperationException("角色管理器没有开启。");
            return provider.HasRoles(roles);
        }

        public static RoleGroup Create(string name)
        {
            if (!IsEnabled)
                throw new InvalidOperationException("角色管理器没有开启。");
            return provider.Create(name);
        }

        public static bool Remove(Guid id)
        {
            if (!IsEnabled)
                throw new InvalidOperationException("角色管理器没有开启。");
            return provider.Remove(id);
        }

        public static bool SetName(Guid id, string newName)
        {
            if (!IsEnabled)
                throw new InvalidOperationException("角色管理器没有开启。");
            return provider.SetName(id, newName);
        }

        public static bool SetParent(Guid targetID, Guid parentID)
        {
            if (!IsEnabled)
                throw new InvalidOperationException("角色管理器没有开启。");
            return provider.SetParent(targetID, parentID);
        }

        public static RoleGroup GetRoleGroup(Guid id)
        {
            if (!IsEnabled)
                throw new InvalidOperationException("角色管理器没有开启。");
            return provider.GetRoleGroup(id);
        }

        public static bool SetRoles(Guid id, string[] roles)
        {
            if (!IsEnabled)
                throw new InvalidOperationException("角色管理器没有开启。");
            return provider.SetRoles(id, roles);
        }
    }
}
