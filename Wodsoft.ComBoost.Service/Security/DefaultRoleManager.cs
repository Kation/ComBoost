using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Security
{
    public class DefaultRoleManager : RoleManagerProvider
    {
        private Intenal.SecurityContext data;

        public DefaultRoleManager()
        {
            data = new Intenal.SecurityContext();
        }

        public override string[] GetRoles()
        {
            var member = MemberManager.GetMemberInfo();
            if (member == null)
                return new string[0];
            return member.Group.Roles.ToArray();
        }

        public override bool HasRoles(string[] roles)
        {
            var data = GetRoles();
            foreach (var role in roles)
                if (!data.Contains(role))
                    return false;
            return true;
        }

        public override RoleGroup Create(string name)
        {
            RoleGroup item = new RoleGroup();
            item.Index = Guid.NewGuid();
            item.Name = name;
            data.RoleGroups.Add(item);
            data.SaveChanges();
            return item;
        }

        public override bool Remove(Guid id)
        {
            return data.Database.ExecuteSqlCommand("DELETE * FROM RoleGroups WHERE [BaseIndex] = {0}", id) > 0;
        }

        public override bool SetName(Guid id, string newName)
        {
            RoleGroup item = data.RoleGroups.Find(id);
            if (item == null)
                return false;
            item.Name = newName;
            data.SaveChanges();
            return true;
        }

        public override bool SetParent(Guid targetID, Guid parentID)
        {
            RoleGroup item = data.RoleGroups.Find(targetID);
            RoleGroup parent = data.RoleGroups.Find(parentID);
            if (item == null)
                return false;
            item.Parent = parent;
            data.SaveChanges();
            return true;
        }

        public override RoleGroup GetRoleGroup(Guid id)
        {
            return data.RoleGroups.Find(id);
        }

        public override bool SetRoles(Guid id, string[] roles)
        {
            RoleGroup item = data.RoleGroups.Find(id);
            if (item == null)
                return false;
            item.Roles = roles;
            data.SaveChanges();
            return true;
        }
    }
}
