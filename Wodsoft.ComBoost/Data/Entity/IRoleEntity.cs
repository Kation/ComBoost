using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Data.Entity
{
    /// <summary>
    /// Entity with role support.
    /// </summary>
    public interface IRoleEntity : IEntity
    {
        /// <summary>
        /// Ensure this user is belong to a role.
        /// </summary>
        /// <param name="role">Role name.</param>
        /// <returns>true if user is member of this role.</returns>
        bool IsInRole(object role);
    }
}
