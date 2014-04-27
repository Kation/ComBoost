using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;

namespace Wodsoft.ComBoost.Business
{
    public class RoleProvider : RoleManagerProvider
    {
        public override string[] GetRoles()
        {
            throw new NotSupportedException();
        }

        public override bool HasRoles(string[] roles)
        {
            if (BussinessApplication.Current.Security ==null)
            return true;
            foreach (var role in roles)
                if (!BussinessApplication.Current.Security.IsInRole(role))
                    return false;
            return true;
        }

        public override RoleGroup Create(string name)
        {
            throw new NotSupportedException();
        }

        public override bool Remove(Guid id)
        {
            throw new NotSupportedException();
        }

        public override bool SetName(Guid id, string newName)
        {
            throw new NotSupportedException();
        }

        public override bool SetParent(Guid targetID, Guid parentID)
        {
            throw new NotSupportedException();
        }

        public override bool SetRoles(Guid id, string[] roles)
        {
            throw new NotSupportedException();
        }

        public override RoleGroup GetRoleGroup(Guid id)
        {
            throw new NotSupportedException();
        }
    }
}
