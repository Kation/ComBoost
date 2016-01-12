using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace System.Data.Entity
{
    /// <summary>
    /// Entity base object for user.
    /// </summary>
    public abstract class UserBase : EntityBase, IPassword, IRoleEntity
    {
        /// <summary>
        /// Get or set the sha1 hashed password.
        /// </summary>
        [Hide(IsHiddenOnEdit = false, IsHiddenOnCreate = false, IsHiddenOnDetail = true)]
        [Required]
        [CustomDataType(CustomDataType.Password)]
        [MaxLength(20)]
        public virtual byte[] Password { get; set; }

        /// <summary>
        /// Get or set the salt data for password.
        /// </summary>
        [MaxLength(6)]
        [Hide]
        [Required]
        public virtual byte[] Salt { get; set; }

        /// <summary>
        /// Set a new password.
        /// </summary>
        /// <param name="password">New password.</param>
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

        /// <summary>
        /// Verify a password is equal to this entity.
        /// </summary>
        /// <param name="password">Password to verify.</param>
        /// <returns>Return true if equal.</returns>
        public virtual bool VerifyPassword(string password)
        {
            using (var sha = System.Security.Cryptography.SHA1.Create())
            {
                var data = sha.ComputeHash(sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password)).Concat(Salt).ToArray());
                for (int i = 0; i < 20; i++)
                    if (data[i] != Password[i])
                        return false;
                return true;
            }
        }

        /// <summary>
        /// Ensure this user is belong to a role.
        /// </summary>
        /// <param name="role">Role name.</param>
        /// <returns>true if user is member of this role.</returns>
        public virtual bool IsInRole(object role)
        {
            return true;
        }
    }
}
