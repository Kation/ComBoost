using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Security
{
    public class DefaultMemberManager : MemberManagerProvider
    {
        private Intenal.SecurityContext data;

        public DefaultMemberManager()
        {
            data = new Intenal.SecurityContext();
        }

        public override bool ChangePassword(string username, string newPassword)
        {
            var item = data.MemberInfos.SingleOrDefault(t => t.Username.ToLower() == username.ToLower());
            if (item == null)
                return false;
            Random rnd = new Random();
            item.Salt= new byte[6];
            rnd.NextBytes(item.Salt);
            using (var sha = new System.Security.Cryptography.SHA1CryptoServiceProvider())
                item.Password = sha.ComputeHash(sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(newPassword)).Concat(item.Salt).ToArray());
            data.SaveChanges();
            return true;
        }

        public override bool Delete(string username)
        {
            var item = data.MemberInfos.SingleOrDefault(t => t.Username == username.ToLower());
            if (item == null)
                return false;
            data.MemberInfos.Remove(item);
            data.SaveChanges();
            return true;
        }

        public override bool Create(string username, string email, string password)
        {
            if (data.MemberInfos.Count(c => c.Username.ToLower() == username.ToLower() || c.Email == email.ToLower()) > 0)
                return false;
            var item = new MemberInfo();
            item.Username = username;
            item.Email = email.ToLower();
            item.Index = Guid.NewGuid();
            item.SetPassword(password);
            data.MemberInfos.Add(item);
            data.SaveChanges();
            return true;
        }

        public override bool ContainsUsername(string username)
        {
            return data.MemberInfos.Count(c => c.Username.ToLower() == username.ToLower()) > 0;
        }

        public override MemberInfo GetMemberInfo(string username)
        {
            return data.MemberInfos.SingleOrDefault(c => c.Username.ToLower() == username.ToLower());
        }

        public override bool Verify(string username, string password)
        {
            var item = data.MemberInfos.SingleOrDefault(t => t.Username == username.ToLower());
            if (item == null)
                return false;
            using (var sha = new System.Security.Cryptography.SHA1CryptoServiceProvider())
            {
                var pwd = sha.ComputeHash(sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password)).Concat(item.Salt).ToArray());
                for (int i = 0; i < 20; i++)
                    if (pwd[i] != item.Password[i])
                        return false;
                return true;
            }
        }

        public override bool Edit(MemberInfo memberInfo)
        {
            data.Entry<MemberInfo>(memberInfo).State = Data.Entity.EntityState.Modified;
            return data.SaveChanges() > 0;
        }
    }
}
