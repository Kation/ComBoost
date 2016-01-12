using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace System.Web.Security
{
    /// <summary>
    /// Extensions of ComBoostPrincipal.
    /// </summary>
    public static class ComBoostPrincipalExtensions
    {
        /// <summary>
        /// Get role entity from principal.
        /// </summary>
        /// <typeparam name="TUser">Type of user.</typeparam>
        /// <param name="principal">Principal.</param>
        /// <returns></returns>
        public static TUser GetUser<TUser>(this IPrincipal principal)
            where TUser : class, IRoleEntity
        {
            ComBoostPrincipal cp = principal as ComBoostPrincipal;
            return GetUser<TUser>(cp);
        }

        /// <summary>
        /// Get role entity from principal.
        /// </summary>
        /// <typeparam name="TUser">Type of user.</typeparam>
        /// <param name="principal">Principal.</param>
        /// <returns></returns>
        public static TUser GetUser<TUser>(this ComBoostPrincipal principal)
            where TUser : class, IRoleEntity
        {
            if (principal == null)
                return null;
            TUser roleEntity = principal.RoleEntity as TUser;
            return roleEntity;
        }

        /// <summary>
        /// Determines whether the current principal belongs to the specified role.
        /// </summary>
        /// <param name="principal">Comboost principal.</param>
        /// <param name="role">Role.</param>
        /// <returns></returns>
        public static bool IsInRole(this IPrincipal principal, object role)
        {
            ComBoostPrincipal item = principal as ComBoostPrincipal;
            if (item == null)
                throw new NotSupportedException("Only support with comboost principal.");
            return item.IsInRole(role);
        }
    }
}
