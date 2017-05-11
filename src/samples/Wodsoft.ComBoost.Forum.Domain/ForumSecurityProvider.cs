using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity;
using Wodsoft.ComBoost.Forum.Core;
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

        protected override async Task<IPermission> GetPermissionByIdentity(string identity)
        {
            var memberContext = DatabaseContext.GetWrappedContext<IMember>();
            var item = await memberContext.GetAsync(identity);
            return item;
        }

        protected override async Task<IPermission> GetPermissionByUsername(string username)
        {
            username = username.ToLower();
            var memberContext = DatabaseContext.GetWrappedContext<IMember>();
            var item = await memberContext.Query().SingleOrDefaultAsync(t => t.Username.ToLower() == username);
            return item;
        }

        public override async Task<IPermission> GetPermissionAsync(IDictionary<string, string> properties)
        {
            var user = (IMember)await base.GetPermissionAsync(properties);
            if (user != null && properties.ContainsKey("password") && !user.VerifyPassword(properties["password"]))
                return null;
            return user;
        }
    }
}
