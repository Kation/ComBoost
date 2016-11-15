using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Data.Entity
{
    public abstract class UserBase : EntityBase, IHavePassword
    {
        [Hide(IsHiddenOnEdit = false, IsHiddenOnCreate = false, IsHiddenOnDetail = true, IsHiddenOnView = true)]
        [CustomDataType(CustomDataType.Password)]
        [Required]
        public byte[] Password { get; set; }

        [Hide]
        [Required]
        public byte[] Salt { get; set; }

        public virtual void SetPassword(string password)
        {
            Random rnd = new Random();
            Salt = new byte[6];
            rnd.NextBytes(Salt);
            using (var sha = System.Security.Cryptography.SHA1.Create())
            {
                Password = sha.ComputeHash(sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password)).Concat(Salt).ToArray());
            }
        }

        public virtual bool VerifyPassword(string password)
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
    }
}
