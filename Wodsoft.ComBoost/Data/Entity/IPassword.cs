using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Data.Entity
{
    /// <summary>
    /// Password entity object interface.
    /// </summary>
    public interface IPassword
    {
        /// <summary>
        /// Set a new password.
        /// </summary>
        /// <param name="password">New password.</param>
        void SetPassword(string password);
        /// <summary>
        /// Verify a password is equal to this entity.
        /// </summary>
        /// <param name="password">Password to verify.</param>
        /// <returns>Return true if equal.</returns>
        bool VerifyPassword(string password);
    }
}
