using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Web.Routing
{
    /// <summary>
    /// User entity route.
    /// </summary>
    public class EntityRoute : Route
    {
        /// <summary>
        /// Initialize entity route.
        /// </summary>
        /// <param name="url">Route url.</param>
        /// <param name="routeHandler">Route handler.</param>
        public EntityRoute(string url, IRouteHandler routeHandler) : base(url, routeHandler) { }

        private Type _UserType;
        /// <summary>
        /// Get or set user entity type.
        /// </summary>
        public Type UserType
        {
            get { return _UserType; }
            set
            {
                if (!typeof(IRoleEntity).IsAssignableFrom(value))
                    throw new ArgumentException("value doesn't inherit IRoleEntity.");
                _UserType = value;
            }
        }
    }
}
