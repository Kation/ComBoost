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
            OriginPrincipal = user;
        }

        public IPrincipal OriginPrincipal { get; private set; }

        public IIdentity Identity
        {
            get { return OriginPrincipal.Identity; }
        }

        public static RoleEntityResolveDelegate Resolve { get; set; }

        public bool IsInRole(string role)
        {
            if (Resolve == null)
                return OriginPrincipal.IsInRole(role);
            RouteData routeData = RouteTable.Routes.GetRouteData(new HttpContextWrapper(HttpContext.Current));
            EntityRoute route = routeData.Route as EntityRoute;
            if (route == null)
                return OriginPrincipal.IsInRole(role);
            IRoleEntity entity = Resolve(route.UserType, Identity.Name);
            if (entity == null)
                return false;
            return entity.IsInRole(role);
        }
    }

    public delegate IRoleEntity RoleEntityResolveDelegate(Type entityType, string username);
}
