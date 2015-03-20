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
    /// <summary>
    /// Entity route collection extension.
    /// </summary>
    public static class EntityRouteCollectionExtensions
    {
        /// <summary>
        /// Maps the specified URL route. 
        /// </summary>
        /// <typeparam name="TUser">Type of user entity.</typeparam>
        /// <param name="routes">A collection of routes for the application.</param>
        /// <param name="name">The name of the route to map.</param>
        /// <param name="url">The URL pattern for the route.</param>
        /// <returns></returns>
        public static Route MapRoute<TUser>(this RouteCollection routes, string name, string url) where TUser : class, IRoleEntity, new()
        {
            return MapRoute<TUser>(routes, name, url, null /* defaults */, (object)null /* constraints */);
        }

        /// <summary>
        /// Maps the specified URL route and sets default route values. 
        /// </summary>
        /// <typeparam name="TUser">Type of user entity.</typeparam>
        /// <param name="routes">A collection of routes for the application.</param>
        /// <param name="name">The name of the route to map.</param>
        /// <param name="url">The URL pattern for the route.</param>
        /// <param name="defaults">An object that contains default route values.</param>
        /// <returns></returns>
        public static Route MapRoute<TUser>(this RouteCollection routes, string name, string url, object defaults) where TUser : class, IRoleEntity, new()
        {
            return MapRoute<TUser>(routes, name, url, defaults, (object)null /* constraints */);
        }

        /// <summary>
        /// Maps the specified URL route and sets default route values and constraints. 
        /// </summary>
        /// <typeparam name="TUser">Type of user entity.</typeparam>
        /// <param name="routes">A collection of routes for the application.</param>
        /// <param name="name">The name of the route to map.</param>
        /// <param name="url">The URL pattern for the route.</param>
        /// <param name="defaults">An object that contains default route values.</param>
        /// <param name="constraints">A set of expressions that specify values for the url parameter.</param>
        /// <returns></returns>
        public static Route MapRoute<TUser>(this RouteCollection routes, string name, string url, object defaults, object constraints) where TUser : class, IRoleEntity, new()
        {
            return MapRoute<TUser>(routes, name, url, defaults, constraints, null /* namespaces */);
        }

        /// <summary>
        /// Maps the specified URL route and sets the namespaces. 
        /// </summary>
        /// <typeparam name="TUser">Type of user entity.</typeparam>
        /// <param name="routes">A collection of routes for the application.</param>
        /// <param name="name">The name of the route to map.</param>
        /// <param name="url">The URL pattern for the route.</param>
        /// <param name="namespaces">A set of namespaces for the application.</param>
        /// <returns></returns>
        public static Route MapRoute<TUser>(this RouteCollection routes, string name, string url, string[] namespaces) where TUser : class, IRoleEntity, new()
        {
            return MapRoute<TUser>(routes, name, url, null /* defaults */, null /* constraints */, namespaces);
        }

        /// <summary>
        /// Maps the specified URL route and sets default route values and namespaces. 
        /// </summary>
        /// <typeparam name="TUser">Type of user entity.</typeparam>
        /// <param name="routes">A collection of routes for the application.</param>
        /// <param name="name">The name of the route to map.</param>
        /// <param name="url">The URL pattern for the route.</param>
        /// <param name="defaults">An object that contains default route values.</param>
        /// <param name="namespaces">A set of namespaces for the application.</param>
        /// <returns></returns>
        public static Route MapRoute<TUser>(this RouteCollection routes, string name, string url, object defaults, string[] namespaces) where TUser : class, IRoleEntity, new()
        {
            return MapRoute<TUser>(routes, name, url, defaults, null /* constraints */, namespaces);
        }

        /// <summary>
        ///  Maps the specified URL route and sets default route values, constraints, and namespaces.  
        /// </summary>
        /// <typeparam name="TUser">Type of user entity.</typeparam>
        /// <param name="routes">A collection of routes for the application.</param>
        /// <param name="name">The name of the route to map.</param>
        /// <param name="url">The URL pattern for the route.</param>
        /// <param name="defaults">An object that contains default route values.</param>
        /// <param name="constraints">A set of expressions that specify values for the url parameter.</param>
        /// <param name="namespaces">A set of namespaces for the application.</param>
        /// <returns></returns>
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
