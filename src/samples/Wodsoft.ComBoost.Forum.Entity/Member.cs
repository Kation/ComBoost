using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity;
using Wodsoft.ComBoost.Forum.Core;

namespace Wodsoft.ComBoost.Forum.Entity
{
    public class Member : EntityBase, IMember, IPermission
    {
        [Searchable]
        public string Username { get; set; }

        public byte[] Password { get; set; }

        public byte[] Salt { get; set; }

        public void SetPassword(string password)
        {
            Random rnd = new Random();
            Salt = new byte[6];
            rnd.NextBytes(Salt);
            using (var sha = System.Security.Cryptography.SHA1.Create())
            {
                Password = sha.ComputeHash(sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password)).Concat(Salt).ToArray());
            }
        }

        public bool VerifyPassword(string password)
        {
            if (Password == null || Salt == null)
                return false;
            using (var sha = System.Security.Cryptography.SHA1.Create())
            {
                var data = sha.ComputeHash(sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password)).Concat(Salt).ToArray());
                for (int i = 0; i < 20; i++)
                    if (data[i] != Password[i])
                        return false;
                return true;
            }
        }

        string IPermission.Identity { get { return Index.ToString(); } }

        string IPermission.Name { get { return Username; } }

        object[] IPermission.GetStaticRoles()
        {
            return new object[0];
        }

        bool IPermission.IsInRole(object role)
        {
            return true;
        }
    }
}
