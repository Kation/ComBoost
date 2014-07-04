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

        public IRoleEntity RoleEntity { get; private set; }
        private bool _IsFailure;

        internal bool InitRoleEntity()
        {
            if (_IsFailure)
                return false;
            if (RoleEntity != null)
                return true;
            EntityRoute route = CurrentRoute.Route as EntityRoute;
            if (route == null)
            {
                _IsFailure = true;
                return false;
            }
            RoleEntity = Resolve(route.UserType, Identity.Name);
            if (RoleEntity == null)
            {
                _IsFailure = true;
                return false;
            }
            return true;
        }

        public bool IsInRole(string role)
        {
            if (!OriginPrincipal.Identity.IsAuthenticated)
                return false;
            if (Resolve == null)
                if (OriginPrincipal == this)
                    return false;
                else
                    return OriginPrincipal.IsInRole(role);
            if (!InitRoleEntity())
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
