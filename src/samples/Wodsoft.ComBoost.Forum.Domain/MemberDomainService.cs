using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity;
using Wodsoft.ComBoost.Forum.Core;
using Wodsoft.ComBoost.Security;

namespace Wodsoft.ComBoost.Forum.Domain
{
    public class MemberDomainService : DomainService
    {
        public async Task SignIn([FromService] IAuthenticationProvider authenticationProvider, [FromValue] string username, [FromValue] string password)
        {
            if (authenticationProvider.GetAuthentication().Identity.IsAuthenticated)
                return;
            bool result = await authenticationProvider.SignInAsync(new Dictionary<string, string>
            {
                { "username", username },
                { "password", password }
            });
            if (!result)
                throw new UnauthorizedAccessException("用户名或密码不正确");
        }

        public async Task SignOut([FromService] IAuthenticationProvider authenticationProvider)
        {
            if (!authenticationProvider.GetAuthentication().Identity.IsAuthenticated)
                return;
            await authenticationProvider.SignOutAsync();
        }

        public async Task SignUp([FromService] IAuthenticationProvider authenticationProvider,
            [FromService] IDatabaseContext databaseContext, [FromValue] string username, [FromValue] string password)
        {
            if (authenticationProvider.GetAuthentication().Identity.IsAuthenticated)
                return;
            if (username.Length < 3)
                throw new ArgumentException("用户名不能小于3位。");
            if (password.Length < 3)
                throw new ArgumentException("密码不能小于3位。");
            username = username.Trim();
            var memberContext = databaseContext.GetWrappedContext<IMember>();
            var count = await memberContext.Query().CountAsync(t => t.Username.ToLower() == username.ToLower());
            if (count != 0)
                throw new ArgumentException("用户名已存在。");
            var member = memberContext.Create();
            //member.Get(t => t.Thread);
            member.Username = username;
            member.SetPassword(password);
            memberContext.Add(member);
            await databaseContext.SaveAsync();
            await authenticationProvider.SignInAsync(member);
        }
    }
}
