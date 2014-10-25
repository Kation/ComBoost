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
    public class ComBoostPrincipal : IPrincipal
    {
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

        public IPrincipal OriginPrincipal { get; private set; }

        public IIdentity Identity { get; private set; }

        public static RoleEntityResolveDelegate Resolve { get; set; }

        public RouteData CurrentRoute { get; private set; }

        private IRoleEntity _RoleEntity;
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
                    _RoleEntity = Resolve(route.UserType, Identity.Name);
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

        public bool IsInRole(string role)
        {
            if (!OriginPrincipal.Identity.IsAuthenticated)
                return false;
            if (Resolve == null)
                if (OriginPrincipal == this)
                    return false;
                else
                    return OriginPrincipal.IsInRole(role);
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
