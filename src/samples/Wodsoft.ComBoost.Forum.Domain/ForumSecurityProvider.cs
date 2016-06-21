using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity;
using Wodsoft.ComBoost.Security;

namespace Wodsoft.ComBoost.Forum.Domain
{
    public class ForumSecurityProvider : GeneralSecurityProvider
    {
        public ForumSecurityProvider(IDatabaseContext databaseContext)
        {
            DatabaseContext = databaseContext;
        }

        public IDatabaseContext DatabaseContext { get; private set; }

        protected override Task<IPermission> GetPermissionByIdentity(string identity)
        {
            throw new NotImplementedException();
        }

        protected override Task<IPermission> GetPermissionByUsername(string username)
        {
            throw new NotImplementedException();
        }
    }
}
