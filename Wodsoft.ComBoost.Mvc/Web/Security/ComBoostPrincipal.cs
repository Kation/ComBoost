using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;

namespace System.Web.Security
{
    /// <summary>
    /// ComBoost principal wrapper.
    /// </summary>
    public class ComBoostPrincipal : IPrincipal
    {
        /// <summary>
        /// Initialize comboost principal.
        /// </summary>
        /// <param name="user">Principal to wrapper.</param>
        public ComBoostPrincipal(IPrincipal user)
        {
            if (user == null)
                throw new ArgumentNullException("user");
            CurrentRoute = RouteTable.Routes.GetRouteData(new HttpContextWrapper(HttpContext.Current));
            if (ComBoostAuthentication.IsEnabled)
            {
                Identity = new ComBoostIdentity(this);
                OriginPrincipal = this;
            }
            else
            {
                Identity = user.Identity;
                OriginPrincipal = user;
            }
        }

        /// <summary>
        /// Get the origin principal.
        /// </summary>
        public IPrincipal OriginPrincipal { get; private set; }

        /// <summary>
        /// Get the identity.
        /// </summary>
        public IIdentity Identity { get; private set; }

        /// <summary>
        /// Get or set the role entity resolve delegate.
        /// Must set this manual or comboost authentication will be failure.
        /// </summary>
        public static RoleEntityResolveDelegate Resolver { get; set; }

        /// <summary>
        /// Get the current route data.
        /// </summary>
        public RouteData CurrentRoute { get; private set; }

        private IRoleEntity _RoleEntity;
        /// <summary>
        /// Get the current user role entity.
        /// </summary>
        public IRoleEntity RoleEntity
        {
            get
            {
                if (_IsFailure)
                    return null;
                if (_RoleEntity == null)
                {
                    if (!Identity.IsAuthenticated)
                    {
                        _IsFailure = true;
                        return null;
                    }
                    if (CurrentRoute == null)
                    {
                        _IsFailure = true;
                        return null;
                    }
                    EntityRoute route = CurrentRoute.Route as EntityRoute;
                    if (route == null)
                    {
                        _IsFailure = true;
                        return null;
                    }
                    _RoleEntity = Resolver(route.UserType, Identity.Name);
                    if (_RoleEntity == null)
                    {
                        _IsFailure = true;
                        return null;
                    }
                }
                return _RoleEntity;
            }
        }
        private bool _IsFailure;

        /// <summary>
        /// Determines whether the current principal belongs to the specified role.
        /// </summary>
        /// <param name="role">Role.</param>
        /// <returns></returns>
        public bool IsInRole(string role)
        {
            if (!OriginPrincipal.Identity.IsAuthenticated)
                return false;
            if (Resolver == null)
                if (OriginPrincipal == this)
                    return false;
                else
                    return OriginPrincipal.IsInRole(role);
            if (RoleEntity == null)
                return false;
            return RoleEntity.IsInRole(role);
        }

        /// <summary>
        /// Determines whether the current principal belongs to the specified role.
        /// </summary>
        /// <param name="role">Role.</param>
        /// <returns></returns>
        public bool IsInRole(object role)
        {
            if (!OriginPrincipal.Identity.IsAuthenticated)
                return false;
            if (Resolver == null)
                throw new NotSupportedException("Resolver is null.");
            if (RoleEntity == null)
                return false;
            return RoleEntity.IsInRole(role);
        }
    }

    /// <summary>
    /// Delegate for getting IRoleEntity.
    /// </summary>
    /// <param name="entityType">Entity type.</param>
    /// <param name="username">Username.</param>
    /// <returns></returns>
    public delegate IRoleEntity RoleEntityResolveDelegate(Type entityType, string username);
}
