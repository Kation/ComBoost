using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Security
{
    public abstract class MemberManagerProvider
    {
        public abstract bool ChangePassword(string username, string newPassword);
        public abstract bool Delete(string username);
        public abstract bool Create(string username, string email, string password);
        public abstract bool ContainsUsername(string username);
        public abstract MemberInfo GetMemberInfo(string username);
        public abstract bool Verify(string username, string password);
        public abstract bool Edit(MemberInfo memberInfo);
    }
}
