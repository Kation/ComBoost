using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;

namespace System.Web.Mvc
{
    public static class EntityRouteCollectionExtensions
    {
        public static Route MapRoute<TUser>(this RouteCollection routes, string name, string url) where TUser : class, IRoleEntity, new()
        {
            return MapRoute<TUser>(routes, name, url, null /* defaults */, (object)null /* constraints */);
        }

        public static Route MapRoute<TUser>(this RouteCollection routes, string name, string url, object defaults) where TUser : class, IRoleEntity, new()
        {
            return MapRoute<TUser>(routes, name, url, defaults, (object)null /* constraints */);
        }

        public static Route MapRoute<TUser>(this RouteCollection routes, string name, string url, object defaults, object constraints) where TUser : class, IRoleEntity, new()
        {
            return MapRoute<TUser>(routes, name, url, defaults, constraints, null /* namespaces */);
        }

        public static Route MapRoute<TUser>(this RouteCollection routes, string name, string url, string[] namespaces) where TUser : class, IRoleEntity, new()
        {
            return MapRoute<TUser>(routes, name, url, null /* defaults */, null /* constraints */, namespaces);
        }

        public static Route MapRoute<TUser>(this RouteCollection routes, string name, string url, object defaults, string[] namespaces) where TUser : class, IRoleEntity, new()
        {
            return MapRoute<TUser>(routes, name, url, defaults, null /* constraints */, namespaces);
        }

        public static Route MapRoute<TUser>(this RouteCollection routes, string name, string url, object defaults, object constraints, string[] namespaces) where TUser : class, IRoleEntity, new()
        {
            if (routes == null)
            {
                throw new ArgumentNullException("routes");
            }
            if (url == null)
            {
                throw new ArgumentNullException("url");
            }

            EntityRoute route = new EntityRoute(url, new MvcRouteHandler())
            {
                Defaults = CreateRouteValueDictionaryUncached(defaults),
                Constraints = CreateRouteValueDictionaryUncached(constraints),
                DataTokens = new RouteValueDictionary(),
                UserType = typeof(TUser)
            };

            Contract.Assert(route != null);
            Contract.Assert(route.Url != null);

            if (route.Constraints != null)
            {
                foreach (var kvp in route.Constraints)
                {
                    if (kvp.Value is string)
                        continue;

                    if (kvp.Value is IRouteConstraint)
                        continue;

                    throw new InvalidOperationException();
                }
            }
            if ((namespaces != null) && (namespaces.Length > 0))
            {
                route.DataTokens["Namespaces"] = namespaces;
            }
            routes.Add(name, route);
            return route;
        }

        /// <summary>
        /// The callers to this method are used at startup only, thus it's a bit better to use
        /// the uncached method because it will run faster for the first few times, and will not
        /// consume memory long term.
        /// </summary>
        private static RouteValueDictionary CreateRouteValueDictionaryUncached(object values)
        {
            var dictionary = values as IDictionary<string, object>;
            if (dictionary != null)
            {
                return new RouteValueDictionary(dictionary);
            }
            RouteValueDictionary d = new RouteValueDictionary();
            if (values != null)
                foreach (var property in values.GetType().GetProperties())
                    d.Add(property.Name, property.GetValue(values));
            return d;
        }
    }
}
