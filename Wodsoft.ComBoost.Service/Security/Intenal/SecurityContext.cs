using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;

namespace System.Security.Intenal
{
    public class SecurityContext : DbContext
    {
        public DbSet<RoleGroup> RoleGroups { get; set; }
        public DbSet<MemberInfo> MemberInfos { get; set; }
    }
}
