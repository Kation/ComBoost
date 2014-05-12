using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Security
{
    public abstract class RoleManagerProvider
    {
        public abstract string[] GetRoles();

        public abstract bool HasRoles(string[] roles);

        public abstract RoleGroup Create(string name);

        public abstract bool Remove(Guid id);

        public abstract bool SetName(Guid id, string newName);

        public abstract bool SetParent(Guid targetID, Guid parentID);

        public abstract bool SetRoles(Guid id, string[] roles);

        public abstract RoleGroup GetRoleGroup(Guid id);
    }
}
